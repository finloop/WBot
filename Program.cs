using Bot.Core;
using System.Collections.Generic;
using System;
using System.Threading;
using Bot.Extensions.MySql;
using Bot.Extensions.Debug;
using Bot.Extensions.CommandLine;

namespace Bot
{
    class Program
    {        static void Main(string[] args)
        {
            CommandLine cmd = new CommandLine(args);
            Channels channels = new Channels();
            channels = FileIO.ReadConfigJson(channels);

            List<string> listchannels = new List<string>();

            for(int i = 0; i < channels.listOfChannels.Count; i++) {
                listchannels.Add(channels.listOfChannels[i].Name);
            }
            List<string> k = MySqlWrapper.MakeQuery("string k");

            // Read config from file
            IRC irc = new IRC(FileIO.ReadConfigParameters("Config.json"), listchannels);
            irc.Connect();
        
            MessageHandler messageHandler = new MessageHandler(irc);

            // Main loop
            while(true)
            {
                // Handle all the messages
                messageHandler.HandleMessage(irc.ReadMessage()); 
            }
        }
    }
}
