﻿using Minecraft_QQ_Core.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Minecraft_QQ_Core.Config
{
    public class ConfigWrite
    {
        public void Ask()
        {
            Save(ConfigFile.自动应答.FullName, Minecraft_QQ.AskConfig);
        }
        public void Command()
        {
            Save(ConfigFile.自定义指令.FullName, Minecraft_QQ.CommandConfig);
        }
        public void Config()
        {
            Save(ConfigFile.主要配置文件.FullName, Minecraft_QQ.MainConfig);
        }
        public void Group()
        {
            Save(ConfigFile.群设置.FullName, Minecraft_QQ.GroupConfig);
        }
        public void Player()
        {
            Save(ConfigFile.玩家储存.FullName, Minecraft_QQ.PlayerConfig);
        }
        public void All()
        {
            Ask();
            Command();
            Config();
            Group();
            Player();
        }

        private void Save(string FileName, object obj)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    File.WriteAllText(FileName,
                    JsonConvert.SerializeObject(obj, Formatting.Indented));

                }
                catch (Exception e)
                {
                    Logs.LogError(e);
                    IMinecraft_QQ.ShowMessageCall?.Invoke("配置" + FileName + "在写入时发发生了错误");
                }
            });
        }
    }
}
