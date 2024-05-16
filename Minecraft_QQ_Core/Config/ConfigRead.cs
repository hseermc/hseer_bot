using Minecraft_QQ_Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Minecraft_QQ_Core.Config;

internal class ConfigRead
{
    /// <summary>
    /// 读取主要配置文件
    /// </summary>
    public static MainConfig ReadConfig()
    {
        Logs.LogOut("[Config]读取主配置");
        try
        {
            var config = JsonConvert.DeserializeObject<MainConfig>
                (File.ReadAllText(ConfigFile.MainConfig.FullName));
            bool save = false;
            if (config == null)
            {
                config = new();
                save = true;
            }
            if (config.Database == null)
            {
                config.Database = new();
                save = true;
            }
            if (config.Check == null)
            {
                config.Check = new();
                save = true;
            }
            if (config.Message == null)
            {
                config.Message = new();
                save = true;
            }
            if (config.Admin == null)
            {
                config.Admin = new();
                save = true;
            }
            if (config.Setting == null)
            {
                config.Setting = new();
                save = true;
            }
            if (config.Socket == null)
            {
                config.Socket = new();
                save = true;
            }
            if (save)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("Mainconfig.json配置文件读取发送错误，已经重写");
                ConfigWrite.Config();
            }
            return config;
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("快去检查你的Mainconfig.json文件语法，用的是json就要遵守语法！");
            Logs.LogError(e);
            return new();
        }
    }
    public static PlayerConfig ReadPlayer()
    {
        Logs.LogOut("[Config]读取玩家配置");
        try
        {
            var config = JsonConvert.DeserializeObject<PlayerConfig>
                (File.ReadAllText(ConfigFile.PlayerConfig.FullName));
            bool save = false;
            if (config == null)
            {
                config = new();
                save = true;
            }
            if (config.PlayerList == null)
            {
                config.PlayerList = [];
                save = true;
            }
            if (config.NotBindList == null)
            {
                config.NotBindList = [];
                save = true;
            }
            if (config.MuteList == null)
            {
                config.MuteList = [];
                save = true;
            }
            if (save)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("Player.json配置文件读取发送错误，已经重写");
                ConfigWrite.Player();
            }
            return config;
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("快去检查你的Player.json文件语法，用的是json就要遵守语法！");
            Logs.LogError(e);
            return new();
        }
    }
    public static GroupConfig ReadGroup()
    {
        Logs.LogOut("[Config]读取群设置");
        try
        {
            var config = JsonConvert.DeserializeObject<GroupConfig>
                (File.ReadAllText(ConfigFile.GroupConfig.FullName));
            bool save = false;
            if (config == null)
            {
                config = new();
                save = true;
            }
            if (config.Groups == null)
            {
                config.Groups = [];
                save = true;
            }
            if (save)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("Group.json配置文件读取发送错误，已经重写");
                ConfigWrite.Group();
            }
            foreach (var item in config.Groups)
            {
                if (item.Value.IsMain == true)
                {
                    Minecraft_QQ.GroupSetMain = item.Key;
                    break;
                }
            }
            return config;
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("快去检查你的Group.json文件语法，用的是json就要遵守语法！");
            Logs.LogError(e);
            return new GroupConfig();
        }
    }
    public static AskConfig ReadAsk()
    {
        Logs.LogOut("[Config]读取自定义应答");
        try
        {
            var config = JsonConvert.DeserializeObject<AskConfig>
                (File.ReadAllText(ConfigFile.AskConfig.FullName));
            bool save = false;
            if (config == null)
            {
                config = new();
                save = true;
            }
            if (config.AskList == null)
            {
                config.AskList = new Dictionary<string, string>()
                {
                    {
                        "服务器菜单",
                        $"服务器查询菜单：{Environment.NewLine}" +
                        $"【{Minecraft_QQ.Main.Check.Head}{Minecraft_QQ.Main.Check.Bind} ID】可以绑定你的游戏ID。{Environment.NewLine}" +
                        $"【{Minecraft_QQ.Main.Check.Head}{Minecraft_QQ.Main.Check.PlayList}】可以查询服务器在线人数。{Environment.NewLine}" +
                        $"【{Minecraft_QQ.Main.Check.Head}{Minecraft_QQ.Main.Check.ServerCheck}】可以查询服务器是否在运行。{Environment.NewLine}" +
                        $"【{Minecraft_QQ.Main.Check.Head}{Minecraft_QQ.Main.Check.Send} 内容】可以向服务器里发送消息。（使用前请确保已经绑定了ID，）"}
                };
                save = true;
            }
            if (save)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("Ask.json配置文件读取发送错误，已经重写");
                File.WriteAllText(ConfigFile.AskConfig.FullName, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            return config;
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("快去检查你的Ask.json文件语法，用的是json就要遵守语法！");
            Logs.LogError(e);
            return new();
        }
    }
    public static CommandConfig ReadCommand()
    {
        Logs.LogOut("[Config]读取自定义指令");
        try
        {
            var config = JsonConvert.DeserializeObject<CommandConfig>
                (File.ReadAllText(ConfigFile.CommandConfig.FullName));
            bool save = false;
            if (config == null)
            {
                config = new();
                save = true;
            }
            if (config.CommandList == null)
            {
                config.CommandList = new()
                {
                    {
                        "help",
                        new()
                        {
                            Command = "qq help",
                            PlayerUse = false,
                            PlayerSend = false
                        }
                    },
                    {
                        "money",
                        new()
                        {
                            Command = "money {arg:name}",
                            PlayerUse = true,
                            PlayerSend = false
                        }
                    },
                    {
                        "mute",
                        new()
                        {
                            Command = "mute {arg:next}",
                            PlayerUse = false,
                            PlayerSend = false
                        }
                    },
                    {
                        "tpa",
                        new()
                        {
                            Command = "tpa {arg:at}",
                            PlayerUse = true,
                            PlayerSend = false
                        }
                    },
                    {
                        "lp",
                        new()
                        {
                            Command = "lp user {arg:at} permission set {arg:next} true",
                            PlayerUse = false,
                            PlayerSend = false
                        }
                    },
                    {
                        "say",
                        new()
                        {
                            Command = "say {arg:x}",
                            PlayerUse = false,
                            PlayerSend = false
                        }
                    }
                };
                save = true;
            }
            if (save)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("Command.json配置文件读取发送错误，已经重写");
                File.WriteAllText(ConfigFile.CommandConfig.FullName, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            return config;
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("快去检查你的Command.json文件语法，用的是json就要遵守语法！");
            Logs.LogError(e);
            return new();
        }
    }
}
