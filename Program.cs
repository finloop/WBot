using Bot.Core;
using System.Collections.Generic;
using System;
using System.Threading;
using Bot.Extensions.MySql;

namespace Bot
{
    class Program
    {        static void Main(string[] args)
        {
            Channels channels = new Channels();
            channels = FileIO.ReadConfigJson(channels);
            List<string> listchannels = new List<string>();

            for(int i = 0; i < channels.listOfChannels.Count; i++) {
                listchannels.Add(channels.listOfChannels[i].Name);
            }
            // Read config from file
            IRC irc = new IRC(FileIO.ReadConfigParameters("Config.json"), listchannels);
            irc.Connect();
            irc.SendChatMessage("preclak", "Działaj!!");
        
            Console.WriteLine("message");
            MessageHandler messageHandler = new MessageHandler(irc);
            List<string> k = MySqlWrapper.MakeQuery("select Name from VIEWERS.POINTSTABLE");

            // Main loop
            while(true)
            {
                // Handle all the messages
                messageHandler.HandleMessage(irc.ReadMessage()); 
            }
        }
    }
}
