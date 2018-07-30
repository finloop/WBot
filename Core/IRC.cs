using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Bot.Core
{
    public class IRC
    {
        private List<String> channels = new List<string>();
        private string botName;
        private string oauth;
        private int port;
        private string ip;

        private TcpClient tcpClient = new TcpClient();
        private StreamReader reader;
        private StreamWriter writer;

        public IRC(List<String> _channels, string _botName, string _oauth, int _port, string _ip)
        {
            channels = _channels;
            botName = _botName;
            oauth = _oauth;
            port = _port;
            ip = _ip;
        }

        public IRC(FileIO.ConfigParameters parameters, List<string> _channels)
        {
            channels = _channels;
            botName = parameters.botName;
            oauth = parameters.oauth;
            port = parameters.port;
            ip = parameters.ip;
        }

        public void Connect()
        {
            try
            {
                //Console.WriteLine("Connecting...");
                tcpClient = new TcpClient(ip, port);
                reader = new StreamReader(tcpClient.GetStream());
                writer = new StreamWriter(tcpClient.GetStream());

                writer.WriteLine("PASS " + oauth);
                //Console.WriteLine("PASS " + oauth);
                writer.WriteLine("NICK " + botName);
               // Console.WriteLine("NICK " + botName);
                writer.WriteLine("USER " + botName + " 8 * :" + botName);
                //Console.WriteLine("USER " + botName + " 8 * :" + botName);

                for (int i = 0; i < channels.Count; i++)
                {
                    writer.WriteLine("JOIN #" + channels[i]);
                }

                writer.WriteLine("CAP REQ :twitch.tv/commands");

                writer.Flush();

            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ConnectTo(string channel)
        {
            if (tcpClient.Connected)
            {
                if (!channels.Contains(channel))
                {
                    SendRawMessage("JOIN #" + channel);
                    channels.Add(channel);
                }

            }
        }

        public void DisconnectFrom(string channel)
        {
            if (tcpClient.Connected)
            {

                if (channels.Contains(channel))
                {
                    SendRawMessage("PART #" + channel);
                    channels.Remove(channel);
                }

            }
        }

        public void SendRawMessage(string message)
        {
            try
            {
                writer.WriteLine(message);
                writer.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Connect();
            }
        }

        public void SendChatMessage(string channel, string message)
        {
            try
            {
                SendRawMessage(":" + botName + "!" + botName + "@" + botName +
                ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Connect();
            }
        }

        public void SendWhisper(string reciever, string message)
        {
            try
            { // PRIVMSG #jtv :/w [TargetUser][whitespace][message]
                SendRawMessage("PRIVMSG jtv :/w " + reciever + " " + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Connect();
            }
        }

        public string ReadMessage()
        {
            try
            {
                string message = reader.ReadLine();
                if (message.Equals("PING :tmi.twitch.tv"))
                {
                    SendRawMessage("PONG :tmi.twitch.tv");
                    return "PONG";
                }
                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Connect();
                return "ERROR";
            }
        }

    }
}
