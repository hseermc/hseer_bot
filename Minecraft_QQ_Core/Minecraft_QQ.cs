using Minecraft_QQ_Core.Config;
using Minecraft_QQ_Core.MySocket;
using Minecraft_QQ_Core.Robot;
using Minecraft_QQ_Core.Utils;
using OneBotSharp.Objs.Message;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Minecraft_QQ_Core;

public static class Minecraft_QQ
{
    /// <summary>
    /// 配置文件路径
    /// </summary>
    public static string Path { get; } = AppContext.BaseDirectory + "Minecraft_QQ/";
    /// <summary>
    /// Mysql启用
    /// </summary>
    public static bool MysqlOK { get; set; } = false;
    /// <summary>
    /// 主群群号
    /// </summary>
    public static long MainGroup { get; set; }
    /// <summary>
    /// 主配置文件
    /// </summary>
    public static MainConfig Config { get; set; }
    /// <summary>
    /// 玩家储存配置
    /// </summary>
    public static PlayerConfig Players { get; set; }
    /// <summary>
    /// 群储存配置
    /// </summary>
    public static GroupConfig Groups { get; set; }
    /// <summary>
    /// 自动应答储存
    /// </summary>
    public static AskConfig Asks { get; set; }
    /// <summary>
    /// 自定义指令
    /// </summary>
    public static CommandConfig Commands { get; set; }

    /// <summary>
    /// QQ号取玩家
    /// </summary>
    /// <param name="qq">qq号</param>
    /// <returns>玩家信息</returns>
    public static PlayerObj? GetPlayer(long qq)
    {
        if (Players.PlayerList.TryGetValue(qq, out var value))
        {
            return value;
        }
        return null;
    }
    /// <summary>
    /// ID取玩家
    /// </summary>
    /// <param name="id">玩家ID</param>
    /// <returns>玩家信息</returns>
    public static PlayerObj? GetPlayer(string id)
    {
        var valueCol = Players.PlayerList.Values.Where(a =>
            a.Name.Equals(id, StringComparison.CurrentCultureIgnoreCase));
        if (valueCol.Any())
        {
            return valueCol.First();
        }
        return null;
    }

    /// <summary>
    /// 设置玩家昵称
    /// </summary>
    /// <param name="qq">qq号</param>
    /// <param name="nick">昵称</param>
    public static void SetNick(long qq, string nick)
    {
        if (Players.PlayerList.ContainsKey(qq) == true)
        {
            Players.PlayerList[qq].Nick = nick;
        }
        else
        {
            Players.PlayerList.Add(qq, new()
            {
                QQ = qq,
                Nick = nick
            });
        }

        if (MysqlOK == true)
        {
            Task.Run(() => DBMysql.AddPlayerAsync(Players.PlayerList[qq]));
        }
        else
        {
            ConfigWrite.Player();
        }
    }

    /// <summary>
    /// 设置玩家ID，如果存在直接修改，不存在创建
    /// </summary>
    /// <param name="qq">qq号</param>
    /// <param name="name">玩家ID</param>
    public static void SetPlayerName(long qq, string name)
    {
        var player = GetPlayer(qq) ?? new();
        player.Name = name;
        player.QQ = qq;
        if (!Players.PlayerList.TryAdd(qq, player))
        {
            Players.PlayerList[qq] = player;
        }
        if (MysqlOK == true)
        {
            Task.Run(() => DBMysql.AddPlayerAsync(player));
        }
        else
        {
            ConfigWrite.Player();
        }
    }

    /// <summary>
    /// 直接设置一个玩家数据
    /// </summary>
    /// <param name="player"></param>
    public static void SetPlayer(PlayerObj player)
    {
        var player1 = GetPlayer(player.QQ) ?? player;
        player1.Name = player.Name;
        player1.QQ = player.QQ;
        player1.Nick = player.Nick;
        player1.IsAdmin = player1.IsAdmin;

        if (!Players.PlayerList.TryAdd(player.QQ, player1))
        {
            Players.PlayerList[player.QQ] = player1;
        }
        if (MysqlOK == true)
        {
            Task.Run(() => DBMysql.AddPlayerAsync(player1));
        }
        else
        {
            ConfigWrite.Player();
        }
    }

    /// <summary>
    /// 禁言玩家
    /// </summary>
    /// <param name="name">名字</param>
    public static void MutePlayer(string name)
    {
        name = name.ToLower();
        if (Players.MuteList.Contains(name) == false)
        {
            Players.MuteList.Add(name);
        }
        if (MysqlOK == true)
        {
            Task.Run(() => DBMysql.AddMuteAsync(name));
        }
        else
        {
            ConfigWrite.Player();
        }
    }

    /// <summary>
    /// 添加禁止绑定
    /// </summary>
    /// <param name="name">名字</param>
    public static void AddNotBind(string name)
    {
        name = name.ToLower();
        if (Players.NotBindList.Contains(name) == false)
        {
            Players.NotBindList.Add(name);
        }
        if (MysqlOK == true)
        {
            Task.Run(() => DBMysql.AddNotBindAsync(name));
        }
        else
        {
            ConfigWrite.Player();
        }
    }

    /// <summary>
    /// 删除禁止绑定
    /// </summary>
    /// <param name="name">名字</param>
    public static void RemoveNotBind(string name)
    {
        name = name.ToLower();
        Players.NotBindList.Remove(name);
        if (MysqlOK == true)
        {
            Task.Run(() => DBMysql.DeleteNotBindAsync(name));
        }
        else
        {
            ConfigWrite.Player();
        }
    }

    /// <summary>
    /// 添加群设置
    /// </summary>
    /// <param name="obj">群信息</param>
    public static void AddGroup(GroupObj obj)
    {
        if (!Groups.Groups.TryAdd(obj.Group, obj))
        {
            Groups.Groups[obj.Group] = obj;
        }

        ConfigWrite.Group();
    }

    /// <summary>
    /// 解除禁言
    /// </summary>
    /// <param name="qq">玩家QQ号</param>
    public static void UnmutePlayer(long qq)
    {
        var player = GetPlayer(qq);
        if (player != null && !string.IsNullOrWhiteSpace(player.Name))
        {
            UnmutePlayer(player.Name);
        }
    }
    /// <summary>
    /// 解除禁言
    /// </summary>
    /// <param name="name">玩家ID</param>
    public static void UnmutePlayer(string name)
    {
        name = name.ToLower();
        Players.MuteList.Remove(name);
        if (MysqlOK == true)
            Task.Run(() => DBMysql.DeleteMuteAsync(name));
        else
            ConfigWrite.Player();
    }

    /// <summary>
    /// 设置维护模式状态
    /// </summary>
    /// <param name="open">状态</param>
    public static void FixModeChange(bool open)
    {
        Config.Setting.FixMode = open;
    }

    /// <summary>
    /// 重载配置
    /// </summary>
    public static bool Reload()
    {
        if (Directory.Exists(Path) == false)
        {
            Directory.CreateDirectory(Path);
        }
        if (!File.Exists(Path + Logs.log))
        {
            try
            {
                File.WriteAllText(Path + Logs.log, $"正在尝试创建日志文件{Environment.NewLine}");
            }
            catch (Exception e)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke($"[Minecraft_QQ]日志文件创建失败{Environment.NewLine}{e}");
                return false;
            }
        }

        ConfigFile.MainConfig = new FileInfo(Path + "Mainconfig.json");
        ConfigFile.PlayerConfig = new FileInfo(Path + "Player.json");
        ConfigFile.AskConfig = new FileInfo(Path + "Ask.json");
        ConfigFile.CommandConfig = new FileInfo(Path + "Command.json");
        ConfigFile.GroupConfig = new FileInfo(Path + "Group.json");

        //读取主配置文件
        ConfigRead.ReadConfig();

        //读取群设置
        ConfigRead.ReadGroup();

        //读自动应答消息
        ConfigRead.ReadAsk();

        //读取玩家数据
        if (Config.Database.Enable == true)
        {
            DBMysql.MysqlStart();
            if (MysqlOK == false)
            {
                Logs.LogOut("[Mysql]Mysql链接失败");
                ConfigRead.ReadPlayer();
            }
            else
            {
                Players = new();
                DBMysql.Load();
                Logs.LogOut("[Mysql]Mysql已连接");
            }
        }
        else
        {
            ConfigRead.ReadPlayer();
        };

        //读取自定义指令
        ConfigRead.ReadCommand();

        return true;
    }
    /// <summary>
    /// 插件启动
    /// </summary>
    public static async Task Start()
    {
        if (!Reload())
            return;

        await Task.Run(() =>
        {
            while (Groups.Groups.Count == 0 || MainGroup == 0)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("请设置QQ群，有且最多一个主群");
                IMinecraft_QQ.ConfigInitCall?.Invoke();
                foreach (var item in Groups.Groups)
                {
                    if (item.Value.IsMain == true)
                    {
                        MainGroup = item.Key;
                        break;
                    }
                }
            }
        });

        ConfigSave.Init();
        RobotCore.Start();
        PluginServer.ServerStop();
        PluginServer.StartServer();
        SendGroup.Start();
        IMinecraft_QQ.IsStart = true;

        RobotCore.SendGroupMessage(MainGroup,
        [
            MsgText.Build($"[Minecraft_QQ]已启动[{IMinecraft_QQ.Version}]")
        ]);
    }

    public static void Stop()
    {
        ConfigSave.Stop();
        IMinecraft_QQ.IsStart = false;
        PluginServer.ServerStop();
        DBMysql.MysqlStop();
        RobotCore.Stop();
        SendGroup.Stop();
    }

    public static void AddAsk(string check, string res)
    {
        if (!Asks.AskList.TryAdd(check, res))
        {
            Asks.AskList[check] = res;
        }

        ConfigWrite.Ask();
    }
}
