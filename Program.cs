using Bot.Core;
using System.Collections.Generic;
using System;
using System.Threading;
using Bot.Extensions.MySql;
using Bot.Extensions.Debug;
using Bot.Extensions.CommandLine;
using Bot.Modules.Points;
using Bot.Extensions.Random;

namespace Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine cmd = new CommandLine(args);
            

            List<Channel> channels = FileIO.ReadConfigJson("Channels.json");
            ModuleManager.channels = channels;
            List<String> listofchannels = new List<string>();

            foreach (Channel channel in channels)
            {
                listofchannels.Add(channel.Name);
            }

            IRC irc = new IRC(FileIO.ReadConfigParameters("Config.json"), listofchannels);
            irc.Connect();

            // Read config from file


            MessageHandler messageHandler = new MessageHandler(irc);

            // Main loop
            while (true)
            {
                // Handle all the messages
                messageHandler.HandleMessage(irc.ReadMessage());
            }
        }
    }
}
