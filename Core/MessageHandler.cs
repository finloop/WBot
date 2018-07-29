using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Core
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

                // Only channel owner can subs

                moduleManager.Handle(channel, msg, sender);
            }
        }
    }
}
