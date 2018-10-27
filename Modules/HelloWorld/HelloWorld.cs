using System.Collections.Generic;
using Bot.Core;
using System;

namespace Bot.Modules.HelloWorld
{
    public class HelloWorld : CommandsModule
    {
        public HelloWorld(IRC _irc) : base(_irc) {
            base.addId("!hello");
            moduleName = "Bot.Modules.HelloWorld.HelloWorld";
        }
            
         override public void HandleMessage(Message message) {
            if(message.msg.StartsWith(getIds()[0]))
                SendChatMessage(message.channel, "HelloWorld!");
        }


        
    }
}