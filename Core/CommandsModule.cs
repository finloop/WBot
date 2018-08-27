using System.Collections.Generic;
using Bot.Core;
using System;

namespace Bot
{
    public class CommandsModule
    {
        #region VARIABLES
        
        private List<string> ids = new List<string>();
        public List<string> ActiveChannels = new List<string>();
        public IRC irc;

        #endregion

        public List<string> getIds() {
            return ids;
        }
        public List<string> getActiveChannels() {
            return ActiveChannels;
        }

        public bool isActiveOnChannel(string channel) {
            for(int i = 0; i < ActiveChannels.Count; i++) {
                if(channel.Equals(ActiveChannels[i]))
                    return true;
            }  
            return false;
        }
        public void SendChatMessage(string channel, string msg) {
            irc.SendChatMessage(channel,msg);
        }

        public void SendWhisper(string channel, string msg) {
            irc.SendWhisper(channel, msg);
        }

        
        public virtual bool AddToChannel(string channel) {
            for(int i = 0; i < ActiveChannels.Count; i++) {
                if(channel.Equals(ActiveChannels[i])) {
                    return false;
                }
            }
            ActiveChannels.Add(channel);
            return true;
        }

        public virtual bool RemoveFromChannel(string channel) {
            for(int i = 0; i < ActiveChannels.Count; i++) {
                if(channel.Equals(ActiveChannels[i])) {
                    ActiveChannels.Remove(channel);
                    return true;
                }
            }
            return false;
        }

        // Override it to add your custom commands etc.
        public virtual void HandleMessage(string channel, string msg, string sender) { }

        public void addId(string id) {
            int i = ids.FindIndex(x => x.Equals(id));
            if(i == -1)
                ids.Add(id);
        }
        
        #region CONSTRUCTORS
        public CommandsModule(List<string> _Ids, List<string> _ActiveChannels, IRC _irc) {
            ids = _Ids;
            ActiveChannels = _ActiveChannels;
            irc = _irc;
        }

        public CommandsModule(List<string> _ActiveChannels, IRC _irc) {
            ActiveChannels = _ActiveChannels;
            irc = _irc;
            //saveState();
        }
        #endregion

    }
}