using Bot.Core;
using System.Collections.Generic;
using Bot.Modules.HelloWorld;
using System;
using System.Threading;
using System.Collections.Concurrent;
using Bot.Modules.Subscribe;
using Bot.Modules.Points;
using Bot.Modules.Utilities;

namespace Bot.Core
{
    public class ModuleManager
    {
        List<CommandsModule> commandsModules = new List<CommandsModule>();

        List<PassiveModule> passiveModules = new List<PassiveModule>();
        public static List<Channel> channels = new List<Channel>();
        public BlockingCollection<Message> messages;
        public Thread worker;
        private IRC irc;
    
        public ModuleManager(IRC _irc)
        {
            InitializeCommandModules(_irc);
            InitializePassiveModules(_irc);
            messages  = new BlockingCollection<Message>();
            worker = new Thread(DoWork);
            worker.IsBackground = true;
            worker.Start();
            irc = _irc;
            Console.WriteLine("Starting worker....");
        }

        private void DoWork() {
            while(true) {
                if(messages.Count >= 1) {
                    Message m;
                    if(messages.TryTake(out m, TimeSpan.FromSeconds(1))) {
                        Handle(m);
                    }

                    
                } else {
                    Thread.Sleep(50);
                }
            }
        }

        private void InitializeCommandModules(IRC irc)
        {
            // Add modules here modules.add(....)
            commandsModules.Add(new HelloWorld(irc));
            commandsModules.Add(new Points(irc));
            commandsModules.Add(new Utilities(irc));
        }

        private void InitializePassiveModules(IRC irc) {
            passiveModules.Add(new Subscribe(this, irc));
        }

        public void Handle(Message message)
        {
            // Check if channel exist on the list if so check command modues
            int channelindex = channels.FindIndex(x => x.Name == message.channel);
            if (channelindex != -1)
            {
                Channel channel = channels[channelindex];
                List<String> activeModules = channel.ActiveModules;
                foreach(String module in activeModules) {
                    int index = commandsModules.FindIndex(x => x.moduleName == module);
                    if(index != -1) {
                        commandsModules[index].HandleMessage(message);
                    }
                }
            }

            for (int i = 0; i < passiveModules.Count; i++)
                {
                    // Check every command in commmand module
                    List<string> list = passiveModules[i].getIds();
                    for (int k = 0; k < list.Count; k++)
                    {
                        if (message.msg.StartsWith(list[k]))
                        {       
                            passiveModules[i].HandleMessage(message.channel, message.msg, message.sender);
                        }
                    }
                }
        }

        public List<string> getCommandModuleIds() {
            List<string> all = new List<string>();
            for (int i = 0; i < commandsModules.Count; i++)
                {
                    // Check every command in commmand module
                    List<string> list = commandsModules[i].getIds();
                    all.AddRange(list);
                }
                return all;
        }

        public List<string> getPassiveModuleIds() {
            List<string> all = new List<string>();
            for (int i = 0; i < passiveModules.Count; i++)
                {
                    // Check every command in commmand module
                    List<string> list = passiveModules[i].getIds();
                    all.AddRange(list);
                }
                return all;
        }

        public List<string> getAllIds() {
            List<string> all = new List<string>();
            all.AddRange(getCommandModuleIds());
            all.AddRange(getPassiveModuleIds());
            return all;
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
                    irc.SendChatMessage(channel, " added " + comm +" to this channel");
                    FileIO.WriteConfigJson(ModuleManager.channels, "Channels.json");
                }

            }
        }

        // Join channel's chat
        public void JoinChannel(string channel)
        {
            int i = channels.FindIndex(x => x.Name.Equals(channel));
            if (i == -1){
                ModuleManager.channels.Add(new Channel(channel));
                FileIO.WriteConfigJson(ModuleManager.channels, "Channels.json");
            }

        }

        public void LeaveChannel(string channel)
        {
            int i = channels.FindIndex(x => x.Name.Equals(channel));
            if (i != -1){
                channels.RemoveAt(channels.FindIndex(x => x.Name.Equals(channel)));
                FileIO.WriteConfigJson(ModuleManager.channels, "Channels.json");
            }
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
                    irc.SendChatMessage(channel, " removed " + comm +" from this channel");
                    FileIO.WriteConfigJson(ModuleManager.channels, "Channels.json");
                }

            }
        }
    }
}