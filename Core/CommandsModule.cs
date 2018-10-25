using System.Collections.Generic;
using Bot.Core;
using System;

namespace Bot
{
    public class CommandsModule
    {
        #region VARIABLES
        
        private List<string> ids = new List<string>();
        public IRC irc;
        public  String moduleName = "null";

        #endregion

        public List<string> getIds() {
            return ids;
        }
        public void SendChatMessage(string channel, string msg) {
            irc.SendChatMessage(channel,msg);
        }

        public void SendWhisper(string channel, string msg) {
            irc.SendWhisper(channel, msg);
        }

        
        public virtual bool AddToChannel(string channel) {
            // not sure what toStrng returns
            int channelIndex = ModuleManager.channels.FindIndex(x => x.Name == channel);
            // find out if channel exists and module is not added to it, if not added add it
            if(channelIndex != -1){
                int moduleIndex = ModuleManager.channels[channelIndex].ActiveModules.FindIndex(x => x == moduleName);
                if(moduleIndex == -1) {
                    ModuleManager.channels[channelIndex].ActiveModules.Add(moduleName);
                    return true;
                }
            }
            return false;

        }

        public virtual bool RemoveFromChannel(string channel) {
           // not sure what toStrng returns
            int channelIndex = ModuleManager.channels.FindIndex(x => x.Name == channel);
            // find out if channel exists and module is not added to it, if not added add it
            if(channelIndex != -1){
                int moduleIndex = ModuleManager.channels[channelIndex].ActiveModules.FindIndex(x => x == moduleName);
                if(moduleIndex != -1) {
                    ModuleManager.channels[channelIndex].ActiveModules.RemoveAt(moduleIndex);
                    return true;
                }
            }
            return false;

        }

        // Override it to add your custom commands etc.
        public virtual void HandleMessage(Message message) { }

        public void addId(string id) {
            int i = ids.FindIndex(x => x.Equals(id));
            if(i == -1)
                ids.Add(id);
        }
        
        #region CONSTRUCTORS
        public CommandsModule(List<string> _Ids, IRC _irc) {
            ids = _Ids;
            irc = _irc;
        }

        public CommandsModule(IRC _irc) {
            irc = _irc;
            //saveState();
        }
        #endregion

    }
}