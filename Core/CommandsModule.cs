using System.Collections.Generic;
using WBot.Core;
using System;

namespace WBot
{
    public class CommandsModule
    {
        private List<string> Ids = new List<string>();
        // TODO Add option to subscribe to module 
        private List<string> ActiveChannels = new List<string>();

        private IRC irc;
        public List<string> getIds() {
            return Ids;
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

        
        public bool AddToChannel(string channel) {
            for(int i = 0; i < ActiveChannels.Count; i++) {
                if(channel.Equals(ActiveChannels[i])) {
                    return false;
                }
            }
            Console.WriteLine(2);
            ActiveChannels.Add(channel);
            return true;
        }

        public bool RemoveFromChannel(string channel) {
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
            Ids.Add(id);
        }
        
        public CommandsModule(List<string> _Ids, List<string> _ActiveChannels, IRC _irc) {
            Ids = _Ids;
            ActiveChannels = _ActiveChannels;
            irc = _irc;
        }

        public CommandsModule(List<string> _ActiveChannels, IRC _irc) {
            ActiveChannels = _ActiveChannels;
            irc = _irc;
            //saveState();
        }

    }
}