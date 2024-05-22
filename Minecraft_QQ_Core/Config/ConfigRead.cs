using System;
using System.IO;
using Minecraft_QQ_Core.Utils;
using Newtonsoft.Json;

namespace Minecraft_QQ_Core.Config;

internal class ConfigRead
{
    /// <summary>
    /// 读取主要配置文件
    /// </summary>
    public static void ReadConfig()
    {
        Logs.LogOut("[Config]读取主配置");
        if (ConfigFile.MainConfig.Exists == false)
        {
            Logs.LogOut("[Config]新建主配置");
            Minecraft_QQ.Config = new MainConfig();
            ConfigWrite.Config();
        }
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

            Minecraft_QQ.Config = config;

            if (save)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("Mainconfig.json配置文件读取失败，已经重写");
                ConfigWrite.Config();
            }
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("配置文件Mainconfig.json读取错误，已经重写");
            Logs.LogError(e);
            Minecraft_QQ.Config = new MainConfig();
            ConfigWrite.Config();
        }
    }
    public static void ReadPlayer()
    {
        Logs.LogOut("[Config]读取玩家配置");

        if (ConfigFile.PlayerConfig.Exists == false)
        {
            Logs.LogOut("[Config]新建玩家信息储存");
            Minecraft_QQ.Players = PlayerConfig.Make();
            ConfigWrite.Player();
            return;
        }
        try
        {
            var config = JsonConvert.DeserializeObject<PlayerConfig>
                (File.ReadAllText(ConfigFile.PlayerConfig.FullName));
            bool save = false;
            if (config == null)
            {
                config = PlayerConfig.Make();
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
                IMinecraft_QQ.ShowMessageCall?.Invoke("Player.json配置文件读取失败，已经重写");
                Minecraft_QQ.Players = config;
                ConfigWrite.Player();
            }
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("配置文件Player.json读取错误，已经重写");
            Logs.LogError(e);
            Minecraft_QQ.Players = PlayerConfig.Make();
            ConfigWrite.Player();
        }
    }
    public static void ReadGroup()
    {
        Logs.LogOut("[Config]读取群设置");
        if (ConfigFile.GroupConfig.Exists == false)
        {
            Logs.LogOut("[Config]新建群设置配置");

            Minecraft_QQ.Groups = new();
            ConfigWrite.Group();
        }
        try
        {
            var config = JsonConvert.DeserializeObject<GroupConfig>
                (File.ReadAllText(ConfigFile.GroupConfig.FullName));
            bool save = false;
            if (config == null || config.Groups == null)
            {
                config = new();
                save = true;
            }

            Minecraft_QQ.Groups = config;

            if (save)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("Group.json配置文件读取失败，已经重写");
                ConfigWrite.Group();
            }
            foreach (var item in config.Groups)
            {
                if (item.Value.IsMain == true)
                {
                    Minecraft_QQ.MainGroup = item.Key;
                    break;
                }
            }
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("配置文件Group.json读取错误，已经重写");
            Logs.LogError(e);
            Minecraft_QQ.Groups = new();
            ConfigWrite.Group();
        }
    }
    public static void ReadAsk()
    {
        Logs.LogOut("[Config]读取自定义应答");
        if (ConfigFile.AskConfig.Exists == false)
        {
            Minecraft_QQ.Asks = AskConfig.Make();
            Logs.LogOut("[Config]新建自定义应答");
            ConfigWrite.Ask();
        }
        try
        {
            var config = JsonConvert.DeserializeObject<AskConfig>
                (File.ReadAllText(ConfigFile.AskConfig.FullName));
            bool save = false;
            if (config == null || config.AskList == null)
            {
                config = AskConfig.Make();
                save = true;
            }

            Minecraft_QQ.Asks = config;

            if (save)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("Ask.json配置文件读取失败，已经重写");
                ConfigWrite.Ask();
            }
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("配置文件Ask.json读取错误，已经重写");
            Logs.LogError(e);
            Minecraft_QQ.Asks = AskConfig.Make();
            ConfigWrite.Ask();
        }
    }
    public static void ReadCommand()
    {
        Logs.LogOut("[Config]读取自定义指令");
        if (ConfigFile.CommandConfig.Exists == false)
        {
            Minecraft_QQ.Commands = CommandConfig.Make();
            Logs.LogOut("[Config]新建自定义指令");
            ConfigWrite.Command();

            return;
        }

        try
        {
            var config = JsonConvert.DeserializeObject<CommandConfig>
                (File.ReadAllText(ConfigFile.CommandConfig.FullName));
            bool save = false;
            if (config == null || config.CommandList == null)
            {
                config = CommandConfig.Make();
                save = true;
            }
            Minecraft_QQ.Commands = config;
            if (save)
            {
                IMinecraft_QQ.ShowMessageCall?.Invoke("Command.json配置文件读取失败，已经重写");
                ConfigWrite.Command();
            }
        }
        catch (Exception e)
        {
            IMinecraft_QQ.ShowMessageCall?.Invoke("配置文件Command.json读取错误，已经重写");
            Logs.LogError(e);
            Minecraft_QQ.Commands = CommandConfig.Make();
            ConfigWrite.Command();
        }
    }
}
