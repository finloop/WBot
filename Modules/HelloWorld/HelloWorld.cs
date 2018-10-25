using System.Collections.Generic;
using Bot.Core;
using System;

namespace Bot.Modules.HelloWorld
{
    public class HelloWorld : CommandsModule
    {
        public HelloWorld(IRC _irc) : base(_irc) {
            base.addId("!hello");
        }
            
         override public void HandleMessage(Message message) {
            SendChatMessage(message.channel, "HelloWorld!");
        }


        
    }
}