using System.Collections.Generic;
using System;
using Bot.Modules.Points;
using Bot.Modules.Utilities;

namespace Bot.Core
{
        public class Channel
        {
            public string Name { get; set; }
            public List<string> ActiveModules { get; set; }
            public PointsConfig pointsConfig { get; set; }
            public UtilitiesConfig utilConfig { get; set; }
            public Channel()
            {
            }
            public Channel(string _channel)
            {
                Name = _channel;
                ActiveModules = new List<string>();
                pointsConfig = new PointsConfig();
                pointsConfig.Name = _channel;
                pointsConfig.pointsName = "points";
                pointsConfig.challengeName = "challenge";
                pointsConfig.challengeAccept = "accept";
                pointsConfig.pointsNameMultiple = "points";
                pointsConfig.rouletteName = "roulette";
                pointsConfig.donateName = "donate";
                utilConfig.uptime = "uptime";
                utilConfig.viewers = "wiewers";
            }
        }

/*         public void AddModuleToChannel(string channel, string moduleName)
        {
            for (int i = 0; i < listOfChannels.Count; i++)
            {
                if (channel.Equals(listOfChannels[i].Name))
                {
                    //Console.WriteLine(listOfChannels[i].ActiveModules.FindIndex(x => x.Equals(moduleName)));
                    if (listOfChannels[i].ActiveModules.FindIndex(x => x.Equals(moduleName)) == -1)
                    {
                        Console.WriteLine("k");
                        listOfChannels[i].ActiveModules.Add(moduleName);

                    }
                }
            }
        }

        public int FindChannel(string channel)
        {
            return listOfChannels.FindIndex(x => x.Name.Equals(channel));
        }
        public int FindModuleIndex(Channel channel, string moduleName)
        {
            return channel.ActiveModules.FindIndex(x => x.Equals(moduleName));
        }
        public void RemoveModuleFromChannel(Channel channel, string moduleName)
        {
            int i = FindModuleIndex(channel, moduleName);
            if (i != -1)
            {
                channel.ActiveModules.Remove(channel.ActiveModules[i]);
            }
        }
        public void RemoveModuleFromChannel(int channelIndex, string moduleName)
        {
            listOfChannels[channelIndex].ActiveModules.Remove(moduleName);
        } */
}