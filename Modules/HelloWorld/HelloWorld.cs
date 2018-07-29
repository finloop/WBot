using System.Collections.Generic;
using Bot.Core;
using System;

namespace Bot.Modules.HelloWorld
{
    public class HelloWorld : CommandsModule
    {
        public HelloWorld(List<string> _ActiveChannels, IRC _irc) : base(_ActiveChannels, _irc) {
            base.addId("!hello");
        }
            
         override public void HandleMessage(string channel, string msg, string sender) {
            SendChatMessage(channel, "HelloWorld!");
        }


        
    }
}