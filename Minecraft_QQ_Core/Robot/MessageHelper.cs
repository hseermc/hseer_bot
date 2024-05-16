using Minecraft_QQ_Core.Config;
using Minecraft_QQ_Core.MySocket;
using Minecraft_QQ_Core.Utils;
using OneBotSharp.Objs.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft_QQ_Core.Robot;

public static class MessageHelper
{
    private static string SetNick(List<MsgBase> msg)
    {
        if (msg.Count != 3)
            return "错误的参数";
        if (msg[1] is MsgAt at && msg[2] is MsgText text)
        {
            if (!long.TryParse(at.Data.QQ, out long qq))
            {
                return "QQ号获取失败";
            }
            string nick = text.Data.Text;
            Minecraft_QQ.SetNick(qq, nick);
            return $"已修改QQ号[{qq}]的昵称为：{nick}";
        }
        else
            return "找不到玩家";
    }
    private static string? SetPlayerName(long fromQQ, List<MsgBase> msg)
    {
        if (msg.Count != 1 || msg[0] is not MsgText text)
        {
            return "错误的参数";
        }
        string data = text.Data.Text;
        if (data.StartsWith(Minecraft_QQ.Main.Check.Head))
        {
            data = data.Replace(Minecraft_QQ.Main.Check.Head, null);
        }
        if (Minecraft_QQ.Main.Setting.CanBind == false)
        {
            return Minecraft_QQ.Main.Message.CantBindText;
        }
        var player = Minecraft_QQ.GetPlayer(fromQQ);
        if (player == null || string.IsNullOrWhiteSpace(player.Name) == true)
        {
            string name = data.Replace(Minecraft_QQ.Main.Check.Bind, "").Trim();
            string check = name.ToLower();
            if (string.IsNullOrWhiteSpace(name) || check.StartsWith("id:")
                || check.StartsWith("id：") || check.StartsWith("id "))
            {
                return "名字无效，请检查";
            }
            else
            {
                if (Minecraft_QQ.Players.NotBindList.Contains(check) == true)
                {
                    return $"禁止绑定名字[{name}]";
                }
                Minecraft_QQ.SetPlayerName(fromQQ, name);
                if (Minecraft_QQ.Main.Setting.SendQQ != 0)
                {
                    RobotCore.SendPrivateMessage(Minecraft_QQ.Main.Setting.SendQQ,
                        [MsgText.Build($"QQ号[{fromQQ}]绑定了名字：[{name}]")]);
                }
                IMinecraft_QQ.GuiCall?.Invoke(GuiCallType.PlayerList);
                return $"绑定名字[{name}]成功！";
            }
        }
        else
            return Minecraft_QQ.Main.Message.AlreadyBindID;
    }

    private static string MutePlayer(List<MsgBase> msg)
    {
        string name;
        if (msg.Count >= 2 && msg[1] is MsgAt at)
        {
            if (!long.TryParse(at.Data.QQ, out long qq))
            {
                return "错误的文本";
            }
            var player = Minecraft_QQ.GetPlayer(qq);
            if (player == null)
            {
                return $"QQ号[{qq}]未绑定名字";
            }
            name = player.Name;
        }
        else if (msg[0] is MsgText text)
        {
            name = text.Data.Text
                .ReplaceFirst(Minecraft_QQ.Main.Check.Head, "")
                .ReplaceFirst(Minecraft_QQ.Main.Admin.Mute, "")
                .Trim();
        }
        else
        {
            return "错误的参数";
        }
        Minecraft_QQ.MutePlayer(name);
        return $"已禁言[{name}]";
    }

    private static string UnmutePlayer(List<MsgBase> msg)
    {
        if (msg.Count > 3)
        {
            return "错误的参数";
        }
        string name;
        if (msg.Count >= 2 && msg[1] is MsgAt at)
        {
            if (!long.TryParse(at.Data.QQ, out long qq))
            {
                return "错误的文本";
            }
            var player = Minecraft_QQ.GetPlayer(qq);
            if (player == null)
            {
                return $"QQ号[{qq}]未绑定名字";
            }
            name = player.Name;
        }
        else if (msg[0] is MsgText text)
        {
            name = text.Data.Text
                .ReplaceFirst(Minecraft_QQ.Main.Check.Head, "")
                .ReplaceFirst(Minecraft_QQ.Main.Admin.UnMute, "")
                .Trim();
        }
        else
        {
            return "错误的参数";
        }
        Minecraft_QQ.UnmutePlayer(name);
        return $"已解禁[{name}]";
    }

    private static string GetPlayerID(List<MsgBase> msg)
    {
        if (msg.Count >= 2 && msg[1] is MsgAt at)
        {
            if (!long.TryParse(at.Data.QQ, out long qq))
            {
                return "错误的格式";
            }
            var player = Minecraft_QQ.GetPlayer(qq);
            if (player == null)
            {
                return $"QQ号[{qq}]未绑定名字";
            }
            else
            {
                return $"QQ号[{qq}]绑定的游戏名为[{player.Name}]";
            }
        }
        else if (msg[0] is MsgText text)
        {
            string data = text.Data.Text
                .ReplaceFirst(Minecraft_QQ.Main.Check.Head, "")
                .ReplaceFirst(Minecraft_QQ.Main.Admin.CheckBind, "")
                .Trim();
            if (long.TryParse(data, out long qq) == false)
            {
                return "无效的QQ号";
            }
            var player = Minecraft_QQ.GetPlayer(qq);
            if (player == null)
            {
                return $"QQ号[{qq}]未绑定名字";
            }
            else
            {
                return $"QQ号[{qq}]绑定的游戏名为[{player.Name}]";
            }
        }
        else
        {
            return "你需要@一个人或者输入它的QQ号来查询";
        }
    }
    private static string RenamePlayer(List<MsgBase> msg)
    {
        if (msg.Count == 3 && msg[1] is MsgAt at && msg[2] is MsgText text)
        {
            if (!long.TryParse(at.Data.QQ, out long qq))
            {
                return "错误的文本";
            }
            string name = text.Data.Text.Trim();
            Minecraft_QQ.SetPlayerName(qq, name);
            return $"已修改QQ号[{qq}]游戏名为[{name}]";
        }
        else
        {
            return "错误的参数";
        }
    }

    private static string GetMuteList()
    {
        if (Minecraft_QQ.Players.MuteList.Count == 0)
            return "没有禁言的玩家";
        else
        {
            string a = "禁言的玩家：";
            foreach (string name in Minecraft_QQ.Players.MuteList)
            {
                a += Environment.NewLine + name;
            }
            return a;
        }
    }
    private static string GetCantBind()
    {
        if (Minecraft_QQ.Players.NotBindList.Count == 0)
        {
            return "没有禁止绑定的名字";
        }
        else
        {
            string a = "禁止绑定的名字列表：";
            foreach (string name in Minecraft_QQ.Players.NotBindList)
            {
                a += Environment.NewLine + name;
            }
            return a;
        }
    }

    private static string FixModeChange()
    {
        string text;
        if (Minecraft_QQ.Main.Setting.FixMode == false)
        {
            Minecraft_QQ.Main.Setting.FixMode = true;
            text = "服务器维护模式已开启";
        }
        else
        {
            Minecraft_QQ.Main.Setting.FixMode = false;
            text = "服务器维护模式已关闭";
        }
        ConfigWrite.Config();
        Logs.LogOut($"[Minecraft_QQ]{text}");
        return text;
    }
    private static string? GetOnlinePlayer(long fromGroup)
    {
        if (Minecraft_QQ.Main.Setting.FixMode)
        {
            return Minecraft_QQ.Main.Message.FixText;
        }
        if (PluginServer.IsReady() == true)
        {
            PluginServer.Send(new()
            {
                group = fromGroup.ToString(),
                command = CommderList.ONLINE
            });
            return null;
        }
        else
        {
            return "发送失败，服务器未准备好";
        }
    }
    private static string? GetOnlineServer(long fromGroup)
    {
        if (Minecraft_QQ.Main.Setting.FixMode)
        {
            return Minecraft_QQ.Main.Message.FixText;
        }
        if (PluginServer.IsReady() == true)
        {
            PluginServer.Send(new()
            {
                group = fromGroup.ToString(),
                command = CommderList.SERVER,
            });
            return null;
        }
        else
        {
            return "发送失败，服务器未准备好";
        }
    }
    private static bool SendCommand(long fromGroup, List<MsgBase> msg, long fromQQ)
    {
        if (msg[0] is not MsgText text)
        {
            return false;
        }
        foreach (var item in Minecraft_QQ.Commands.CommandList)
        {
            string head = text.Data.Text
                .ReplaceFirst(Minecraft_QQ.Main.Check.Head, "");
            if (!head.StartsWith(item.Key))
            {
                continue;
            }
            if (!PluginServer.IsReady())
            {
                RobotCore.SendGroupMessage(fromGroup,
                [
                    MsgAt.BuildAt(fromQQ.ToString()),
                    MsgText.Build("发送失败，服务器未准备好")
                ]);
                return true;
            }
            bool haveserver = false;
            List<string>? servers = null;
            if (item.Value.Servers != null && item.Value.Servers.Count != 0)
            {
                servers = [];
                foreach (var item1 in item.Value.Servers)
                {
                    if (PluginServer.MCServers.ContainsKey(item1))
                    {
                        servers.Add(item1);
                        haveserver = true;
                    }
                }
            }
            else
            {
                haveserver = true;
            }
            if (!haveserver)
            {
                RobotCore.SendGroupMessage(fromGroup,
                [
                    MsgAt.BuildAt(fromQQ.ToString()),
                    MsgText.Build("发送失败，对应的服务器未连接")
                ]);
                return true;
            }
            var player = Minecraft_QQ.GetPlayer(fromQQ);
            if (player == null)
            {
                RobotCore.SendGroupMessage(fromGroup,
                [
                    MsgAt.BuildAt(fromQQ.ToString()),
                    MsgText.Build("你未绑定ID")
                ]);
                return true;
            }
            if (!item.Value.PlayerUse && !player.IsAdmin)
            {
                return true;
            }

            var cmds = item.Value.Command.Split(' ');
            int index1 = 1, index3 = 1;
            string[]? nowlist = head.Split(' ');
            for (int index2 = 0; index2 < cmds.Length; index2++)
            {
                var item1 = cmds[index2];
                if (item1 == "{arg:at}")
                {
                    if (index1 >= msg.Count || msg[index1] is not MsgAt at)
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build("错误的指令参数")
                        ]);

                        return true;
                    }
                    long qq = long.Parse(at.Data.QQ!);
                    var player1 = Minecraft_QQ.GetPlayer(qq);
                    if (player1 == null)
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build($"错误，QQ号{qq}没有绑定游戏名")
                        ]);
                        return true;
                    }
                    cmds[index2] = player1.Name;
                    nowlist = null;
                    index1++;
                }
                else if (item1 == "{arg:atqq}")
                {
                    if (index1 >= msg.Count || msg[index1] is not MsgAt at)
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build("错误的指令参数")
                        ]);

                        return true;
                    }
                    cmds[index2] = at.Data.QQ!;
                    nowlist = null;
                    index1++;
                }
                else if (item1 == "{arg:name}")
                {
                    cmds[index2] = player.Name;
                }
                else if (item1 == "{arg:qq}")
                {
                    cmds[index2] = $"{player.QQ}";
                }
                else if (item1 == "{arg:next}")
                {
                    if (nowlist == null || index3 >= nowlist.Length )
                    {
                        index1++;
                        if (index1 >= msg.Count || msg[index1] is not MsgText text1)
                        {
                            RobotCore.SendGroupMessage(fromGroup,
                            [
                                MsgAt.BuildAt(fromQQ.ToString()),
                                MsgText.Build("错误的指令参数")
                            ]);

                            return true;
                        }
                        nowlist = text1.Data.Text.Split(' ');
                        index3 = 0;
                    }

                    cmds[index2] = nowlist[index3];
                    index3++;
                }
                else if (item1 == "{arg:x}")
                {
                    var builder = new StringBuilder();
                    if (nowlist != null)
                    {
                        for (int a = index3; a < nowlist.Length; a++)
                        {
                            builder.Append(nowlist[a]).Append(' ');
                        }
                    }
                    for (int a = index1; a < msg.Count; a++)
                    {
                        builder.Append(msg[a].ToString()).Append(' ');
                    }

                    cmds[index2] = builder.ToString().Trim();
                }
            }

            var builder1 = new StringBuilder();
            foreach (var item1 in cmds)
            {
                builder1.Append(item1).Append(' ');
            }

            PluginServer.Send(new TranObj
            {
                group = fromGroup.ToString(),
                command = builder1.ToString().Trim(),
                isCommand = true,
                player = item.Value.PlayerSend ? player.Name : CommderList.COMM
            }, servers);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 向服务器发送群消息
    /// </summary>
    /// <param name="group"></param>
    /// <param name="player"></param>
    /// <param name="msg"></param>
    /// <param name="list"></param>
    private static async void SendMessage(GroupObj group, PlayerObj? player, string msg, List<MsgBase> list)
    {
        var config = Minecraft_QQ.Main;
        if (msg.StartsWith(config.Check.Head) && !config.Setting.SendCommand)
        {
            return;
        }

        if (player != null && !Minecraft_QQ.Players.MuteList.Contains(player.Name.ToLower())
                && !string.IsNullOrWhiteSpace(player.Name))
        {
            var builder = new StringBuilder();
            if (list[0] is MsgText text)
            {
                builder.Append(text.ToString());
            }
            else
            {
                builder.Append(await Funtion.GetRich(list[0]) ?? list[0].ToString());
            }
            if (list.Count > 1)
            {
                for (int a = 1; a < list.Count; a++)
                {
                    if (list[a] is MsgText text1)
                    {
                        if (text1.Data.Text.StartsWith("[分享] 不支持的消息类型，请到手机上查看"))
                        {
                            continue;
                        }
                    }
                    builder.Append(list[a].ToString());
                }
            }
            var send = builder.ToString();
            if (!config.Setting.ColorEnable)
            {
                send = Funtion.RemoveColorCodes(send);
            }
            if (!string.IsNullOrWhiteSpace(send))
            {
                var messagelist = new TranObj()
                {
                    group = group.Group.ToString(),
                    message = send,
                    player = !Minecraft_QQ.Main.Setting.SendNickServer ?
                    player.Name : string.IsNullOrWhiteSpace(player.Nick) ?
                    player.Name : player.Nick,
                    command = CommderList.SPEAK
                };
                PluginServer.Send(messagelist);
            }
        }
    }

    /// <summary>
    /// Type=2 群消息。
    /// </summary>
    /// <param name="fromGroup">来源群号。</param>
    /// <param name="fromQQ">来源QQ。</param>
    /// <param name="msg">消息内容。</param>
    public static void GroupMessage(long fromGroup, long fromQQ, string raw, List<MsgBase> msg)
    {
        if (IMinecraft_QQ.IsStart == false)
            return;
        Logs.LogOut($"[{fromGroup}][QQ:{fromQQ}]:{raw}");
        var config = Minecraft_QQ.Main;
        if (Minecraft_QQ.Groups.Groups.ContainsKey(fromGroup) == true)
        {
            var group = Minecraft_QQ.Groups.Groups[fromGroup];
            var player = Minecraft_QQ.GetPlayer(fromQQ);

            //SendMessage(group, player, raw, msg);

            //始终发送
            if (config.Setting.AutoSend && !config.Setting.FixMode && PluginServer.IsReady() 
                && group.EnableSay && player != null)
            {
                SendMessage(group, player, raw, msg);
            }
            if (raw.StartsWith(Minecraft_QQ.Main.Check.Head) && group.EnableCommand == true)
            {
                if (msg[0] is not MsgText text)
                {
                    return;
                }
                string msg_low = text.ToString()[Minecraft_QQ.Main.Check.Head.Length..];
                if (Minecraft_QQ.Main.Setting.AutoSend == false && msg_low.StartsWith(Minecraft_QQ.Main.Check.Send))
                {
                    if (group.EnableSay == false)
                    {
                        RobotCore.SendGroupMessage(fromGroup, [MsgText.Build("该群没有开启聊天功能")]);
                    }
                    else if (Minecraft_QQ.Main.Setting.FixMode)
                    {
                        if (!string.IsNullOrWhiteSpace(Minecraft_QQ.Main.Message.FixText))
                        {
                            RobotCore.SendGroupMessage(fromGroup, 
                            [
                                MsgAt.BuildAt(fromQQ.ToString()),
                                MsgText.Build(Minecraft_QQ.Main.Message.FixText)
                            ]);
                        }
                    }
                    else if (PluginServer.IsReady() == false)
                    {
                        RobotCore.SendGroupMessage(fromGroup, 
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build("发送失败，没有服务器链接")
                        ]);
                    }
                    else if (player == null || string.IsNullOrWhiteSpace(player.Name))
                    {
                        if (!string.IsNullOrWhiteSpace(Minecraft_QQ.Main.Message.NoneBindID))
                        {
                            RobotCore.SendGroupMessage(fromGroup, 
                            [
                                MsgAt.BuildAt(fromQQ.ToString()),
                                MsgText.Build(Minecraft_QQ.Main.Message.NoneBindID)
                            ]);
                        }
                        return;
                    }
                    else if (Minecraft_QQ.Players.MuteList.Contains(player.Name.ToLower()))
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build("你已被禁言")
                        ]);
                    }
                    else
                    {
                        try
                        {
                            string msg_copy = raw;
                            msg_copy = msg_copy.Replace(Minecraft_QQ.Main.Check.Send, "");
                            if (Minecraft_QQ.Main.Setting.ColorEnable == false)
                                msg_copy = Funtion.RemoveColorCodes(msg_copy);
                            if (string.IsNullOrWhiteSpace(msg_copy) == false)
                            {
                                var messagelist = new TranObj()
                                {
                                    group = DataType.group,
                                    message = msg_copy,
                                    player = !Minecraft_QQ.Main.Setting.SendNickServer ?
                                    player.Name : string.IsNullOrWhiteSpace(player.Nick) ?
                                    player.Name : player.Nick,
                                    command = CommderList.SPEAK
                                };
                                PluginServer.Send(messagelist);
                            }
                        }
                        catch (Exception e)
                        {
                            Logs.LogError(e);
                        }
                    }
                }
                else if (player != null && player.IsAdmin == true)
                {
                    if (msg_low.StartsWith(Minecraft_QQ.Main.Admin.Mute))
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build(MutePlayer(msg))
                        ]);
                        return;
                    }
                    else if (msg_low.StartsWith(Minecraft_QQ.Main.Admin.UnMute))
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build(UnmutePlayer(msg))
                        ]);
                        return;
                    }
                    else if (msg_low.StartsWith(Minecraft_QQ.Main.Admin.CheckBind))
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build(GetPlayerID(msg))
                        ]);
                        return;
                    }
                    else if (msg_low.StartsWith(Minecraft_QQ.Main.Admin.Rename))
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build(RenamePlayer(msg))
                        ]);
                        return;
                    }
                    else if (msg_low == Minecraft_QQ.Main.Admin.Fix)
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build(FixModeChange())
                        ]);
                        return;
                    }
                    else if (msg_low == Minecraft_QQ.Main.Admin.GetMuteList)
                    {
                        RobotCore.SendGroupMessage(fromGroup, [MsgText.Build(GetMuteList())]);
                        return;
                    }
                    else if (msg_low == Minecraft_QQ.Main.Admin.GetCantBindList)
                    {
                        RobotCore.SendGroupMessage(fromGroup, [MsgText.Build(GetCantBind())]);
                        return;
                    }
                    else if (msg_low == Minecraft_QQ.Main.Admin.Reload)
                    {
                        RobotCore.SendGroupMessage(fromGroup, [MsgText.Build("开始重读配置文件")]);
                        Minecraft_QQ.Reload();
                        RobotCore.SendGroupMessage(fromGroup, [MsgText.Build("重读完成")]);
                        return;
                    }
                    else if (msg_low.StartsWith(Minecraft_QQ.Main.Admin.Nick))
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build(SetNick(msg)),
                        ]);
                        return;
                    }
                }
                if (msg_low == Minecraft_QQ.Main.Check.PlayList)
                {
                    var test = GetOnlinePlayer(fromGroup);
                    if (test != null)
                        RobotCore.SendGroupMessage(fromGroup, [MsgText.Build(test)]);
                }
                else if (msg_low == Minecraft_QQ.Main.Check.ServerCheck)
                {
                    var test = GetOnlineServer(fromGroup);
                    if (test != null)
                        RobotCore.SendGroupMessage(fromGroup, [MsgText.Build(test)]);
                }

                else if (msg_low.StartsWith(Minecraft_QQ.Main.Check.Bind))
                {
                    var str = SetPlayerName(fromQQ, msg);
                    if (str != null)
                    {
                        RobotCore.SendGroupMessage(fromGroup,
                        [
                            MsgAt.BuildAt(fromQQ.ToString()),
                            MsgText.Build(str)
                        ]);
                    }
                }
                else if (SendCommand(fromGroup, msg, fromQQ) == true)
                {

                }
                else if (Minecraft_QQ.Main.Setting.AskEnable
                    && Minecraft_QQ.Asks.AskList.ContainsKey(msg_low) == true)
                {
                    string message = Minecraft_QQ.Asks.AskList[msg_low];
                    if (string.IsNullOrWhiteSpace(message) == false)
                    {
                        RobotCore.SendGroupMessage(fromGroup, [MsgText.Build(message)]);
                    }
                }
                else if (string.IsNullOrWhiteSpace(Minecraft_QQ.Main.Message.UnknowText) == false)
                {
                    RobotCore.SendGroupMessage(fromGroup, [MsgText.Build(Minecraft_QQ.Main.Message.UnknowText)]);
                }
            }
        }
    }
}
