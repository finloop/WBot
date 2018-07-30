using System.Collections.Generic;
using System;

namespace Bot.Core
{
    public class Channels
    {
        public List<Channel> listOfChannels = new List<Channel>();
        public partial class Channel
        {
            public string Name { get; set; }
            public List<string> ActiveModules { get; set; }

            public Channel(string _channel) {
                Name = _channel;
                ActiveModules = new List<string>();
            }
        }

        public void AddModuleToChannel(string channel, string moduleName) {
            for(int i = 0; i < listOfChannels.Count; i++) {
                if(channel.Equals(listOfChannels[i].Name)) {
                    //Console.WriteLine(listOfChannels[i].ActiveModules.FindIndex(x => x.Equals(moduleName)));
                    if (listOfChannels[i].ActiveModules.FindIndex(x => x.Equals(moduleName)) == -1) {
                        Console.WriteLine("k");
                        listOfChannels[i].ActiveModules.Add(moduleName);
                        
                    }
                }
            }
        }

        public int FindChannel(string channel) {
            return listOfChannels.FindIndex(x => x.Name.Equals(channel));
        }

        public int FindModuleIndex(Channel channel, string moduleName) {
            return channel.ActiveModules.FindIndex(x => x.Equals(moduleName));
        }

        public void RemoveModuleFromChannel(Channel channel, string moduleName) {
            int i = FindModuleIndex(channel, moduleName);
            if (i!= -1) {
                channel.ActiveModules.Remove(channel.ActiveModules[i]);
            }
        }

        public void RemoveModuleFromChannel(int channelIndex, string moduleName) {
            listOfChannels[channelIndex].ActiveModules.Remove(moduleName);
        }
    }
}