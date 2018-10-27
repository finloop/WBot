using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Core
{
    class StringTokenizer
    {
        // returns [sender, channel, message]
        public static List<string> TokenizeChatMsg(string message)
        {  
            List<string> vs = new List<string>();
            int intIndexParseSign = message.IndexOf('!');
            vs.Add(message.Substring(1, intIndexParseSign - 1));                                                            
            intIndexParseSign = message.IndexOf(" :");
            if(message.Contains("PRIVMSG")) {
                int len = intIndexParseSign - message.IndexOf("#") - 1;
                vs.Add(message.Substring(message.IndexOf("#") + 1, len));
                vs.Add(message.Substring(intIndexParseSign + 2));
                return vs;
            } else {
                int len = intIndexParseSign - message.IndexOf("WHISPER ") - 8;
                vs.Add(message.Substring(message.IndexOf("WHISPER ") + 8, len));
                vs.Add(message.Substring(intIndexParseSign + 2));
                return vs;
            }

        }
    }
}
