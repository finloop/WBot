using System.Collections.Generic;
using WBot.Core;
using System;

namespace WBot.Modules.HelloWorld
{
    public class HelloWorld : CommandsModule
    {
        public HelloWorld(List<string> _ActiveChannels, IRC _irc) : base(_ActiveChannels, _irc) {
            base.addId("!hello");
            base.AddToChannel("preclak");
        }
            
         override public void HandleMessage(string channel, string msg, string sender) {
            SendChatMessage(channel, "HelloWorld!");
        }


        
    }
}