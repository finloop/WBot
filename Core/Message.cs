using System.Collections.Generic;

namespace Bot.Core
{
    public class Message {
            public enum InputId {
                irc, whisper
            }
            public string sender;
            public string channel;
            public string msg;
            public InputId inputId;

            public Message(List<string> res, InputId id) {
                sender = res[0];
                channel = res[1];
                msg = res[2];
                inputId = id;
            }
        }
}