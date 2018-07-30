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
            _irc = irc;
            moduleManager = new ModuleManager(_irc);
        }
        public void HandleMessage(string message)
        {
            Console.WriteLine(message);

            if (message.Contains("PRIVMSG") | message.Contains("WHISPER"))
            {
                Message msg = new Message(StringTokenizer.TokenizeChatMsg(message));
                moduleManager.messages.TryAdd(msg, 100);      
            }
        }

        
    }
}
