﻿using Minecraft_QQ.Config;
using Minecraft_QQ.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minecraft_QQ.MySocket
{
    internal class MySocketServer
    {
        public static Dictionary<Socket, Thread> Clients = new Dictionary<Socket, Thread>();
        public static Dictionary<string, Socket> MCServers = new Dictionary<string, Socket>();

        private static Socket ServerSocket;
        private static Thread ServerThread;

        public static bool Start;
        private static void SetState(bool st)
        {
            Start = st;
            Minecraft_QQ.SetWindow?.InitServerList();
        }

        public static bool IsReady()
        {
            return Clients.Count != 0;
        }

        public static void StartServer()
        {
            try
            {
                logs.LogWrite("[INFO][Socket]正在启动端口");
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(Minecraft_QQ.MainConfig.链接.地址);
                ServerSocket.Bind(new IPEndPoint(ip, Minecraft_QQ.MainConfig.链接.端口));
                ServerSocket.Listen(5);

                ServerThread = new Thread(ListenClientConnect);
                ServerThread.Start();
                SetState(true);
                if (Minecraft_QQ.MainConfig.设置.发送日志到主群)
                    IMinecraft_QQ.SGroupMessage(Minecraft_QQ.GroupSetMain, "[Minecraft_QQ]端口已启动\n" +
                        "已绑定在：" + Minecraft_QQ.MainConfig.链接.地址 + ":" + Minecraft_QQ.MainConfig.链接.端口);
                logs.LogWrite("[INFO][Socket]端口已启动");
            }
            catch (Exception exception)
            {
                IMinecraft_QQ.SGroupMessage(Minecraft_QQ.GroupSetMain, "[Minecraft_QQ]启动失败，请看日志" +
                    "\n酷Q/Minecraft_QQ/logs.log");
                logs.LogWrite("[ERROR][Socket]端口启动失败\n" + exception.Message);
                SetState(false);
            }
        }
        private static void ListenClientConnect()
        {
            try
            {
                while (true)
                {
                    Socket clientScoket = ServerSocket.Accept();
                    var readThread = new Thread(ReceiveData);
                    readThread.Start(clientScoket);
                    Clients.Add(clientScoket, readThread);
                    GC.Collect();
                    Thread.Sleep(1000);                          
                    if (!Start)
                    {
                        if (ServerSocket != null)
                            ServerSocket.Close();
                        return;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }
        private static string Receive(Socket socket)
        {
            string data = "";

            byte[] bytes;
            int len = socket.Available;
            if (len > 0)
            {
                bytes = new byte[len];
                int receiveNumber = socket.Receive(bytes);

                if (Minecraft_QQ.MainConfig.链接.编码 == Code.UTF8)
                    data = Encoding.UTF8.GetString(bytes, 0, receiveNumber);
                else
                    data = Encoding.Default.GetString(bytes, 0, receiveNumber);
            }
            return data;
        }

        private static void ReceiveData(dynamic socket)
        {
            bool isCheck = false;
            while (true)
            {
                try
                {
                    string str = Receive(socket);
                    if (!str.Equals(""))
                    {
                        Task.Factory.StartNew(() =>
                        {
                            if (!isCheck)
                            {
                                var temp = Message.MessageCheck(str);
                                if (temp != null)
                                {
                                    if (Minecraft_QQ.MainConfig.设置.发送日志到主群)
                                    {
                                        IMinecraft_QQ.SGroupMessage(Minecraft_QQ.GroupSetMain, "[Minecraft_QQ]服务器" + temp + "已连接");
                                    }
                                    logs.LogWrite("[INFO][Socket]服务器" + temp + "已连接");
                                    if (MCServers.ContainsKey(temp))
                                    {
                                        Close(MCServers[temp], true);
                                        MCServers.Remove(temp);
                                    }
                                    MCServers.Add(temp, socket);
                                }
                                else if (Minecraft_QQ.MainConfig.设置.发送日志到主群)
                                {
                                    IMinecraft_QQ.SGroupMessage(Minecraft_QQ.GroupSetMain, "[Minecraft_QQ]服务器已连接");
                                }
                                isCheck = true;
                                Minecraft_QQ.SetWindow?.InitServerList();
                            }
                            else
                                Message.MessageDo(str);
                        });
                    }
                }
                catch (Exception e)
                {
                    string keys = MCServers.Where(q => q.Value == socket).Select(q => q.Key).FirstOrDefault();
                    if (Minecraft_QQ.MainConfig.设置.发送日志到主群)
                        IMinecraft_QQ.SGroupMessage(Minecraft_QQ.GroupSetMain, "[Minecraft_QQ]服务器" + keys + "连接已断开");
                    logs.LogWrite("[INFO][Socket]服务器" + keys + "连接已断开");
                    Close(socket, true);
                    GC.Collect();
                    return;
                }

                if (!Start)
                {
                    IMinecraft_QQ.SGroupMessage(Minecraft_QQ.GroupSetMain, "线程已关闭");
                    Close(socket, true);
                    return;
                }
                Thread.Sleep(10);      // 延时0.01秒后再接收客户端发送的消息
            }
        }
        private static void Close(Socket socket, bool re)
        {
            if (socket != null)
                socket.Close();
            if (Clients.ContainsKey(socket))
            {
                Clients[socket].Abort();
            }
            Clients.Remove(socket);
            if (re)
                Minecraft_QQ.SetWindow?.InitServerList();
        }
        public static void Close(string name)
        {
            if (MCServers.ContainsKey(name))
            {
                var temp = MCServers[name];
                temp.Close();
                if (Clients.ContainsKey(temp))
                {
                    Clients[temp].Abort();
                }
                Clients.Remove(temp);
                Minecraft_QQ.SetWindow?.InitServerList();
            }
        }

        public static void Send(MessageObj info, List<string> servers = null)
        {
            if (Clients.Count != 0)
            {

                JObject jsonData = new JObject(
                                new JProperty("group", info.group),
                                new JProperty("message", info.message),
                                new JProperty("player", info.player),
                                new JProperty("commder", info.commder),
                                new JProperty("is_commder", info.is_commder));
                if (servers != null)
                {
                    foreach (var temp in servers)
                    {
                        SendData(MCServers[temp], jsonData.ToString());
                    }
                }
                else
                {
                    foreach (Socket socket in new List<Socket>(Clients.Keys))
                    {
                        SendData(socket, jsonData.ToString());
                    }
                }
            }
            else
                IMinecraft_QQ.SGroupMessage(Minecraft_QQ.GroupSetMain, "[Minecraft_QQ]服务器未连接，无法发送");
        }
        private static void SendData(Socket socket, string data)
        {
            try
            {
                if (socket != null && data != null && !data.Equals(""))
                {
                    data = Minecraft_QQ.MainConfig.链接.数据头 + data + Minecraft_QQ.MainConfig.链接.数据尾;
                    byte[] bytes = null;
                    if (Minecraft_QQ.MainConfig.链接.编码 == Code.UTF8)
                        bytes = Encoding.UTF8.GetBytes(data);
                    else
                        bytes = Encoding.Default.GetBytes(data);
                    socket.Send(bytes);
                }
            }
            catch (Exception e)
            {
                Close(socket, true);
                GC.Collect();
                if (Clients.Count == 0)
                    IMinecraft_QQ.SGroupMessage(Minecraft_QQ.GroupSetMain, "[Minecraft_QQ]连接已断开，无法发送\n" + e.Message);
            }
        }
        public static void ServerStop()
        {
            var temp = new Dictionary<Socket, Thread>(Clients);
            foreach (var item in temp)
            {
                Close(item.Key, false);
                if (item.Value != null)
                {
                    item.Value.Abort();
                }
            }
            if (ServerSocket != null)
            {
                ServerSocket.Close();
                ServerSocket = null;
            }
            if (ServerThread != null)
            {
                ServerThread.Abort();
                ServerThread = null;
            }
            Start = false;
        }
    }
}
