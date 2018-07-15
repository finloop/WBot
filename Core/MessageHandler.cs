using System;
using System.Collections.Generic;
using System.Text;

namespace WBot.Core
{
    class MessageHandler
    {
        private ModuleManager moduleManager;

        private IRC _irc;

        public MessageHandler(IRC irc)
        {
            moduleManager = new ModuleManager(irc);
            _irc = irc;
        }
        public void HandleMessage(string message)
        {
            Console.WriteLine(message);

            if (message.Contains("PRIVMSG") | message.Contains("WHISPER"))
            {
                

                string[] res = StringTokenizer.TokenizeChatMsg(message);

                string sender = res[0];
                string channel = res[1];
                string msg = res[2];

                // Only channel owner can sub 
                if (msg.StartsWith("!sub"))
                {
                    string[] temp = msg.Split(' ');
                    if(temp.Length >= 2) {
                        for(int i = 1; i < temp.Length; i++) {
                            moduleManager.AddCommToChannel(channel, temp[i]);
                        }
                    }

                    else {
                        _irc.ConnectTo(sender);
                        moduleManager.AddChannel(sender);
                    }
                    
                } else if (msg.StartsWith("!unsub") /* & sender.Equals(channel) */) {
                    string[] temp = msg.Split(' ');
                    if(temp.Length >= 2) {
                        for(int i = 1; i < temp.Length; i++) {
                            moduleManager.RemoveCommFromChannel(channel, temp[i]);
                        }
                    }
                    else {
                        _irc.DisconnectFrom(channel);
                        moduleManager.RemoveChannel(channel);
                    }
                }
                
                moduleManager.Handle(channel, msg, sender);
            }
        }
    }
}
