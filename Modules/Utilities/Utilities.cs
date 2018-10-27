using System.Collections.Generic;
using Bot.Core;
using System;
using Bot.Extensions.Stream;

namespace Bot.Modules.Utilities
{
    public class Utilities : CommandsModule
    {
        public Utilities(IRC _irc) : base(_irc) {
            moduleName = "Bot.Modules.HelloWorld.Utilities";
            base.addId("!uptime");
            base.addId("!viewers");
        }
            
         override public void HandleMessage(Message message) {
            Channel channel = ModuleManager.channels.Find(x => x.Name == message.channel);

            string uptime ="!" +channel.utilConfig.uptime;
            string viewers ="!" +channel.utilConfig.viewers;

            if(message.msg.StartsWith(uptime)) {
                TimeSpan timeSpan = CheckStream.Uptime(message.channel);
                message.msg = (timeSpan.Hours-2) + " hours " + timeSpan.Minutes + " minutes.";
                irc.SendResponse(message);
            } else if(message.msg.StartsWith(viewers)) {
                List<string> chatters = Chatters.GetViewers(message.channel);
                if(chatters != null)
                if(chatters.Count > 20) {
                    string response = "Widzowie: ";
                    for(int i = 0; i < 10; i++) {
                        response += chatters[i] + ", ";
                    }
                    response += "... jeszcze " + (chatters.Count-10) + ".";
                    message.msg = response;
                    irc.SendResponse(message);
                } else {
                    string response = "Widzowie: ";
                    for(int i = 0; i < chatters.Count; i++) {
                        if(i != (chatters.Count-1))
                            response += chatters[i] + ", ";
                        else 
                            response += chatters[i] + ".";
                    }
                    message.msg = response;
                    irc.SendResponse(message);
                }

            }
        }


        
    }
}