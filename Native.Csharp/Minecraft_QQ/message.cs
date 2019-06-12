﻿using Native.Csharp.App;
using Newtonsoft.Json.Linq;

namespace Color_yr.Minecraft_QQ
{
    class message
    {
        public static string Head;
        public static string End;

        public static messagelist Message_re(string read)
        {
            messagelist messagelist = new messagelist();
            JObject jsonData = JObject.Parse(read);
            if (jsonData.ContainsKey("message"))
            {
                JObject data = (JObject)jsonData["message"];
                messagelist.group = data["group"].ToString();
                messagelist.message = data["message"].ToString();
                messagelist.player = data["player"].ToString();
                messagelist.is_commder = false;
            }
            else if (jsonData.ContainsKey("commder"))
            {
                JObject data = (JObject)jsonData["commder"];
                messagelist.group = data["group"].ToString();
                messagelist.player = data["player"].ToString();
                messagelist.is_commder = false;
            }
            return messagelist;
        }

        public static void Message(string read)
        {
            while (read.IndexOf(Head) == 0 && read.IndexOf(End) != -1)
            {
                use use = new use();
                string buff = use.get_string(read, Head, End);
                buff = use.RemoveColorCodes(buff);
                messagelist messagelist = Message_re(buff);
                if (messagelist.is_commder == false)
                {
                    Common.CqApi.SendGroupMessage(Minecraft_QQ.GroupSet1, buff);
                    if (Minecraft_QQ.GroupSet2 != 0 && Minecraft_QQ.Group2_on == true)
                    {
                        Common.CqApi.SendGroupMessage(Minecraft_QQ.GroupSet2, buff);
                    }
                    if (Minecraft_QQ.GroupSet3 != 0 && Minecraft_QQ.Group3_on == true)
                    {
                        Common.CqApi.SendGroupMessage(Minecraft_QQ.GroupSet3, buff);
                    }
                }
                else if (Minecraft_QQ.Group == 1)
                {
                    Common.CqApi.SendGroupMessage(Minecraft_QQ.GroupSet1, buff);
                    Minecraft_QQ.Group = 0;
                }
                else if (Minecraft_QQ.Group == 2)
                {
                    Common.CqApi.SendGroupMessage(Minecraft_QQ.GroupSet2, buff);
                    Minecraft_QQ.Group = 0;
                }
                else if (Minecraft_QQ.Group == 3)
                {
                    Common.CqApi.SendGroupMessage(Minecraft_QQ.GroupSet3, buff);
                    Minecraft_QQ.Group = 0;
                }
                int i = read.IndexOf(End);
                read = read.Substring(i + End.Length);
            }
        }
    }
}
