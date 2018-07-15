using WBot.Core;
using System.Collections.Generic;
using System;

namespace WBot
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
