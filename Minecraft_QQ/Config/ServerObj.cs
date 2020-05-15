﻿using System.Collections.Generic;

namespace Minecraft_QQ.Config
{
    public class ConfigOBJ
    {
        public MessageOBJ Join { get; set; }
        public MessageOBJ Quit { get; set; }
        public MessageOBJ ChangeServer { get; set; }
        public ServerSetOBJ ServerSet { get; set; }
        public Dictionary<string, string> Servers { get; set; }
        public SendAllServerOBJ SendAllServer { get; set; }
        public SystemOBJ System { get; set; }
        public UserOBJ User { get; set; }
        public LogsOBJ Logs { get; set; }
        public PlaceholderOBJ Placeholder { get; set; }
        public LanguageOBJ Language { get; set; }
        public List<string> Mute { get; set; }
        public string Version { get; set; }
    }
    public class MessageOBJ
    {
        public string Message { get; set; }
        public bool sendQQ { get; set; }
    }
    public class ServerSetOBJ
    {
        public string ServerName { get; set; }
        public string Check { get; set; }
        public string Message { get; set; }
        public string Say { get; set; }
        public int Mode { get; set; }
        public bool SendOneByOne { get; set; }
        public string SendOneByOneMessage { get; set; }
        public bool HideEmptyServer { get; set; }
        public string PlayerListMessage { get; set; }
        public string ServerOnlineMessage { get; set; }
        public bool BungeeCord { get; set; }
        public int CommandDelay { get; set; }
    }
    public class SendAllServerOBJ
    {
        public bool Enable { get; set; }
        public string Message { get; set; }
        public bool OnlySideServer { get; set; }
    }
    public class SystemOBJ
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public bool AutoConnect { get; set; }
        public int AutoConnectTime { get; set; }
        public bool Debug { get; set; }
        public string Head { get; set; }
        public string End { get; set; }
        public int Sleep { get; set; }
    }
    public class UserOBJ
    {
        public bool SendSucceed { get; set; }
        public bool NotSendCommand { get; set; }
    }
    public class LogsOBJ
    {
        public bool Group { get; set; }
        public bool Server { get; set; }
    }
    public class PlaceholderOBJ
    {
        public string Message { get; set; }
        public string Player { get; set; }
        public string ServerName { get; set; }
        public string Server { get; set; }
        public string PlayerNumber { get; set; }
        public string PlayerList { get; set; }
    }
    public class LanguageOBJ
    {
        public string MessageOFF { get; set; }
        public string MessageON { get; set; }
        public string SucceedMessage { get; set; }
    }
}
