using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Core
{
    class StringTokenizer
    {
        // returns [sender, channel, message]
        public static string[] TokenizeChatMsg(string message)
        {
        
            string[] vs = new string[3];
            int intIndexParseSign = message.IndexOf('!');
            vs[0] = message.Substring(1, intIndexParseSign - 1);                                                            
            intIndexParseSign = message.IndexOf(" :");
            int len = intIndexParseSign - message.IndexOf("#") - 1;
            vs[1] = message.Substring(message.IndexOf("#") + 1, len);
            vs[2] = message.Substring(intIndexParseSign + 2);
            return vs;
        }
    }
}
