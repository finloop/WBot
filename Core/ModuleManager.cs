using Bot.Core;
using System.Collections.Generic;
using Bot.Modules.HelloWorld;
using System;

namespace Bot.Core
{
    public class ModuleManager
    {
        List<CommandsModule> commandsModules = new List<CommandsModule>();

        List<PassiveModule> PassiveModules = new List<PassiveModule>();
        Channels channels = new Channels();
        public ModuleManager(IRC _irc)
        {
            InitializeCommandModules(_irc);

        }

        private void InitializeCommandModules(IRC irc)
        {

            // Add modules here modules.add(....)
            channels = FileIO.ReadConfigJson(channels);
            commandsModules.Add(new HelloWorld(getActiveChannels(typeof(HelloWorld).FullName), irc));
        }

        public List<string> getActiveChannels(string moduleName) {
            List<string> temp = new List<string>();
            for(int i = 0; i < channels.listOfChannels.Count; i++) {
                if(channels.listOfChannels[i].ActiveModules.FindIndex(x => x.Equals(moduleName)) != -1) {
                    temp.Add(channels.listOfChannels[i].Name);
                }
            }
            for(int i = 0; i < temp.Count; i++) {
                Console.WriteLine(temp[i]);
            }
            return temp;
        }

        public void Handle(string channel, string msg, string sender)
        {

            // Check if channel exist on the list if so check command modues
            int channelindex = channels.FindChannel(channel);
            if (channelindex != -1)
            {
                // Check every commandModule 
                for (int i = 0; i < commandsModules.Count; i++)
                {
                    Console.WriteLine("1");
                    // Check every command in commmand module
                    List<string> list = commandsModules[i].getIds();
                    for (int k = 0; k < list.Count; k++)
                    {
                        Console.WriteLine("2");
                        if (msg.StartsWith(list[k]) & ((channels.FindModuleIndex(channels.listOfChannels[channelindex], commandsModules[i].GetType().ToString())) != -1))
                        {
                            Console.WriteLine("3");
                            commandsModules[i].HandleMessage(channel, msg, sender);
                        }
                    }

                }
            }

        }

        public void AddCommToChannel(string channel, string comm)
        {
            for (int i = 0; i < commandsModules.Count; i++)
            {
                int k = commandsModules[i].getIds().FindIndex(x => x.Equals(comm));
                // if module has this command
                if (k != -1)
                {
                    commandsModules[i].AddToChannel(channel);
                }

            }
            UpdateChannelsData(channel);
        }

        // Join channel's chat
        public void AddChannel(string channel)
        {
            int i = channels.listOfChannels.FindIndex(x => x.Name.Equals(channel));
            if (i == -1)
                channels.listOfChannels.Add(new Channels.Channel(channel));
        }

        public void RemoveChannel(string channel)
        {
            int i = channels.listOfChannels.FindIndex(x => x.Name.Equals(channel));
            if (i != -1)
                channels.listOfChannels.RemoveAt(channels.listOfChannels.FindIndex(x => x.Name.Equals(channel)));
        }

        public void RemoveCommFromChannel(string channel, string comm)
        {
            for (int i = 0; i < commandsModules.Count; i++)
            {
                int k = commandsModules[i].getIds().FindIndex(x => x.Equals(comm));
                // if module has this command
                if (k != -1)
                {
                    commandsModules[i].RemoveFromChannel(channel);
                }

            }
            UpdateChannelsData(channel);
        }




        private void UpdateChannelsData(string channel)
        {
            int i = channels.listOfChannels.FindIndex(x => x.Name.Equals(channel));
            if (i != -1)
            {
                channels.listOfChannels[i].ActiveModules.Clear();
                // Check if modules contain channel name
                for (int k = 0; k < commandsModules.Count; k++)
                {
                    int m = commandsModules[k].getActiveChannels().FindIndex(x => x.Equals(channel));
                    if (m != -1)
                    {
                        Console.WriteLine("Updating channels data....");
                        channels.AddModuleToChannel(channel, commandsModules[k].GetType().ToString());
                    }
                }
            }
            Console.WriteLine("Finished updatking channels...");
            FileIO.WriteConfigJson(channels);
        }
    }
}