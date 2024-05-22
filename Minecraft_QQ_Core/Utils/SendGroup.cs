using Minecraft_QQ_Core.Robot;
using OneBotSharp.Objs.Message;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Minecraft_QQ_Core.Utils;

public record SendObj
{
    public long Group { get; set; }
    public string Message { get; set; }
}
public static class SendGroup
{
    private static Thread SendT;
    private static bool Run;
    private static List<SendObj> SendList { get; set; } = new();

    public static void AddSend(SendObj obj)
    {
        SendList.Add(obj);
    }

    private static void SendToGroup()
    {
        while (Run)
        {
            if (SendList.Count != 0)
            {
                var group = SendList.First().Group;
                var temp = new StringBuilder();
                lock (SendList)
                {
                    var SendList_C = SendList.Where(a => a.Group == group);
                    var have = false;
                    foreach (var a in SendList_C)
                    {
                        if (string.IsNullOrWhiteSpace(a.Message) == false)
                        {
                            have = true;
                            temp.AppendLine(a.Message);
                        }
                    }
                    if (have)
                    {
                        RobotCore.SendGroupMessage(group, [MsgText.Build(temp.ToString().Trim())]);
                    }
                    SendList.RemoveAll(a => a.Group == group);
                }
            }
            Thread.Sleep(Minecraft_QQ.Config.Setting.SendDelay);
        }
    }
    public static void Start()
    {
        Run = true;
        SendT = new(SendToGroup);
        SendT.Start();
    }

    public static void Stop()
    {
        Run = false;
    }
}