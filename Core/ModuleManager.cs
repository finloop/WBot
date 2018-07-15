using WBot.Core;
using System.Collections.Generic;
using WBot.Modules.HelloWorld;
using System;

namespace WBot.Core
{
    public class ModuleManager
    {
        List<CommandsModule> commandsModules = new List<CommandsModule>();
        Channels channels = new Channels();

        public void Handle(string channel, string msg, string sender)
        {

            // Check if channel exist on the list if so check command modues
            int channelindex = channels.FindChannel(channel);
            if (channelindex != -1)
            {
                // Check every commandModule 
                for (int i = 0; i < commandsModules.Count; i++)
                {
                    // Check every command in commmand module
                    List<string> list = commandsModules[i].getIds();
                    for (int k = 0; k < list.Count; k++)
                    {
                        if (msg.StartsWith(list[k]) & ((channels.FindModuleIndex(channels.listOfChannels[channelindex], commandsModules[i].GetType().ToString())) != -1))
                        {
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

        public ModuleManager(IRC _irc)
        {
            InitializeModules(_irc);

        }

        private void InitializeModules(IRC irc)
        {

            // Add modules here modules.add(....)
            channels = FileIO.ReadConfigJson(channels);
            commandsModules.Add(new HelloWorld(new List<string>(), irc));
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
                        Console.WriteLine(3);
                        channels.AddCommToChannel(channel, commandsModules[k].GetType().ToString());
                    }
                }
            }
            FileIO.WriteConfigJson(channels);
        }
    }
}