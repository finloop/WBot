using Bot.Core;
using System.Collections.Generic;
using System;
using System.Threading;
using Bot.Extensions.MySql;
using Bot.Extensions.Debug;
using Bot.Extensions.CommandLine;
using Bot.Modules.Points;

namespace Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine cmd = new CommandLine(args);
            Channels channels = new Channels();
            channels = FileIO.ReadConfigJson(channels);

            List<string> listchannels = new List<string>();

            for (int i = 0; i < channels.listOfChannels.Count; i++)
            {
                listchannels.Add(channels.listOfChannels[i].Name);
            }
            
            // Read config from file
            IRC irc = new IRC(FileIO.ReadConfigParameters("Config.json"), listchannels);
            irc.Connect();
            string sb = string.Format("use VIEWERS; CREATE TABLE `{0}` (`Name` text COLLATE utf8mb4_unicode_ci,`Points` int(11) DEFAULT NULL,`TotalPoints` int(11) DEFAULT NULL) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;", "lordozopl");

                MySqlWrapper.MakeQuery(sb);


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
