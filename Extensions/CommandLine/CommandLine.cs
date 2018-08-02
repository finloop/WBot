using Bot.Core;
using System;
using Bot.Extensions.Debug;
using Bot.Extensions.MySql;

namespace Bot.Extensions.CommandLine
{
    public class CommandLine
    {
        public CommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
                HandleCommand(args[i]);
        }

        private void HandleCommand(string command)
        {
            if (command.ToLower().Equals("-help"))
            {
                Log.WriteLine("Avalible commands:\n-help - Displays help \n-generate-example - Genrates example files: Config.json, Channels.json"
                + "\n");
            }
            else if (command.ToLower().Equals("-generate-example"))
            {
                string sb = "{\"listOfChannels\":[{\"Name\":\"MyChannel1\",\"ActiveModules\":[\"Bot.Modules.HelloWorld.HelloWorld\"]},{\"Name\":\"MyChannel2\",\"ActiveModules\":[\"Bot.Modules.HelloWorld.HelloWorld\"]}]}";
                string sb1 = "{\"botName\":\"YourName\",\"oauth\":\"oauth:youroauthfrom https://twitchapps.com/tokengen/\",\"port\": 6667,\"ip\": \"irc.twitch.tv\"}";
                FileIO.WriteString("Channels.json", sb);
                FileIO.WriteConfigJson(new MySqlConfig());
                FileIO.WriteString("Config.json",sb1);
            }

        }
    }
}