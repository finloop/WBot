using System.Collections.Generic;

namespace Bot.Core
{
    public class Message {
            public string sender;
            public string channel;
            public string msg;


            public Message(List<string> res) {
                sender = res[0];
                channel = res[1];
                msg = res[2];
            }
        }
}