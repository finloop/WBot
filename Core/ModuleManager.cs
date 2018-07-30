using Bot.Core;
using System.Collections.Generic;
using Bot.Modules.HelloWorld;
using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Bot.Core
{
    public class ModuleManager
    {
        List<CommandsModule> commandsModules = new List<CommandsModule>();

        List<PassiveModule> PassiveModules = new List<PassiveModule>();
        public Channels channels = new Channels();
        public BlockingCollection<Message> messages;
        public Thread worker;
    
        public ModuleManager(IRC _irc)
        {
            InitializeCommandModules(_irc);
            messages  = new BlockingCollection<Message>();
            worker = new Thread(DoWork);
            worker.IsBackground = true;
            worker.Start();

            Console.WriteLine("Starting worker....");
        }

        private void DoWork() {
            while(true) {
                if(messages.Count >= 1) {
                    Message m;
                    if(messages.TryTake(out m, TimeSpan.FromSeconds(1)));
                        Handle(m);
                    
                }
            }
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

            /* 
             * DEBUG: print list of channels implementing module
             *
            for(int i = 0; i < temp.Count; i++) {
                Console.WriteLine(temp[i]);
            } 
            */
            return temp;
        }

        public void Handle(Message message)
        {
            // Check if channel exist on the list if so check command modues
            int channelindex = channels.FindChannel(message.channel);
            if (channelindex != -1)
            {
                // Check every commandModule 
                for (int i = 0; i < commandsModules.Count; i++)
                {

                    // Check every command in commmand module
                    List<string> list = commandsModules[i].getIds();
                    for (int k = 0; k < list.Count; k++)
                    {

                        if (message.msg.StartsWith(list[k]) & ((channels.FindModuleIndex(channels.listOfChannels[channelindex], commandsModules[i].GetType().ToString())) != -1))
                        {

                            commandsModules[i].HandleMessage(message.channel, message.msg, message.sender);
                        }
                    }

                }
            }

        }

        public void AddModuleToChannel(string channel, string comm)
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
        public void JoinChannel(string channel)
        {
            int i = channels.listOfChannels.FindIndex(x => x.Name.Equals(channel));
            if (i == -1)
                channels.listOfChannels.Add(new Channels.Channel(channel));
        }

        public void LeaveChannel(string channel)
        {
            int i = channels.listOfChannels.FindIndex(x => x.Name.Equals(channel));
            if (i != -1)
                channels.listOfChannels.RemoveAt(channels.listOfChannels.FindIndex(x => x.Name.Equals(channel)));
        }

        public void RemoveModuleFromChannel(string channel, string comm)
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
            Console.WriteLine("Finished updating channels...");
            FileIO.WriteConfigJson(channels);
        }
    }
}