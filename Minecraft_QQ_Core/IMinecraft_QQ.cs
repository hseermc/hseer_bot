﻿namespace Minecraft_QQ_Core;

public enum GuiCallType
{
    ServerState, ServerList, PlayerList
}
public class IMinecraft_QQ
{
    public const string Version = "5.0.0";

    /// <summary>
    /// 已经启动
    /// </summary>
    public static bool IsStart = false;

    public delegate void ShowMessage(string message);
    public delegate void ServerConfig(string name, string config);
    public delegate void ConfigInit();
    public delegate void Gui(GuiCallType dofun);
    public delegate void Log(string message);

    public static ShowMessage ShowMessageCall { get; set; }
    public static ConfigInit ConfigInitCall { get; set; }
    public static Gui GuiCall { get; set; }
    public static Log LogCall { get; set; }
}
