﻿using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System.Threading.Tasks;

namespace Color_yr.Minecraft_QQ.Event
{
    public class Event_GroupMessage : IGroupMessage
    {
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            IMinecraft_QQ.RGroupMessage(e.FromGroup.Id, e.FromQQ.Id, e.Message.ToSendString());
        }
    }
}
