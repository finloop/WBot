using System.Collections.Generic;
using Bot.Core;

namespace Bot.Core
{
    public class PassiveModule 
    {
        public List<string> ids;

        public IRC irc;

        public ModuleManager moduleManager;
        public PassiveModule(ModuleManager _moduleManager, IRC _irc) {
            moduleManager = _moduleManager;
            irc = _irc;
            ids = new List<string>();
        }

        public PassiveModule(ModuleManager _moduleManager, IRC _irc, List<string> _ids) {
            moduleManager = _moduleManager;
            irc = _irc;
            ids = _ids;
        }

         // Override it to add your custom commands etc.
        public virtual void HandleMessage(string channel, string msg, string sender) { }

        public void addId(string id) {
            ids.Add(id);
        }

        public void SendChatMessage(string channel, string msg) {
            irc.SendChatMessage(channel,msg);
        }

        public void SendWhisper(string channel, string msg) {
            irc.SendWhisper(channel, msg);
        }

        public List<string> getIds() {
            return ids;
        }

    }
}