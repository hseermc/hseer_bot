namespace Minecraft_QQ_Core.Config;

public class ConfigWrite
{
    public static void Ask()
    {
        ConfigSave.AddItem(new()
        {
            Name = "ask",
            Local = ConfigFile.AskConfig.FullName,
            Obj = Minecraft_QQ.Asks
        });
    }
    public static void Command()
    {
        ConfigSave.AddItem(new()
        {
            Name = "command",
            Local = ConfigFile.CommandConfig.FullName,
            Obj = Minecraft_QQ.Commands
        });
    }
    public static void Config()
    {
        ConfigSave.AddItem(new()
        {
            Name = "config",
            Local = ConfigFile.MainConfig.FullName,
            Obj = Minecraft_QQ.Config
        });
    }
    public static void Group()
    {
        ConfigSave.AddItem(new()
        {
            Name = "group",
            Local = ConfigFile.GroupConfig.FullName,
            Obj = Minecraft_QQ.Groups
        });
    }
    public static void Player()
    {
        ConfigSave.AddItem(new()
        {
            Name = "player",
            Local = ConfigFile.PlayerConfig.FullName,
            Obj = Minecraft_QQ.Players
        });
    }
    public static void All()
    {
        Ask();
        Command();
        Config();
        Group();
        Player();
    }
}
