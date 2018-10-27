using System;
using System.Collections.Generic;
using Bot.Core;
using Bot.Extensions.MySql;
using Bot.Extensions.Debug;
using System.Threading;
using Bot.Extensions.Stream;
using Bot.Extensions.Random;

namespace Bot.Modules.Points
{
    public class PointsCommands
    {
        public static void HandleStartChallenge(Message message, IRC irc) {
            // pattern is: !challenge user int(points)
            // m[1] - user 
            // [2] - points
            String channel = message.channel;
            String msg = message.msg;
            String sender = message.sender; 
            Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);

            string[] m = msg.Split(" ");
            int points;
            if(m.Length >= 3) {
                if(!Points.isUserChallenged(channel, m[1]) && Int32.TryParse(m[2], out points)) {
                    if(points > 0) {
                        if(points > Points.getPoints(channel, sender)) {
                            // send message to the guy who wanted to start
                            message.msg = string.Format("{0} ma za mało {1}", sender, channelinfo.pointsConfig.pointsNameMultiple);
                            message.channel = sender;
                            irc.SendResponse(message);
                            return;
                        }
                        if(points > Points.getPoints(channel, m[1])) {
                            // send message to the guy who wanted to start
                            //irc.SendChatMessage(channel, string.Format("{0} ma za mało {1}", m[1], channelinfo.pointsConfig.pointsNameMultiple));
                            message.msg = string.Format("{0} ma za mało {1}", m[1], channelinfo.pointsConfig.pointsNameMultiple);
                            message.channel = sender;
                            irc.SendResponse(message);
                            return;
                        }

                        Points.setChallenger(channel, sender, m[1], points);
                        message.msg = string.Format("{0} wyzwał Cię pojedynek, wpisz !{2} na kanale {3} aby akceptować pojedynek", sender, m[1], channelinfo.pointsConfig.challengeAccept, message.channel);
                        message.sender = m[1];
                        irc.SendResponse(message);
                        StartCancelThread(channel, m[1], 30000);
                        return;
                    }

                } else {
                    //irc.SendChatMessage(channel, "Coś poszło nie tak monkaS");
                    return;
                }
            } else {
                //irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");
                return;
            }
        }

        public static void HandleEndChallenge(string channel, string sender, string msg, IRC irc) {
            string whostartedchall = Points.getChallenger(channel, sender);
            Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);
            if(!whostartedchall.Equals("")) {
                int r = RandomN.getRandomUnsignedIntFromRange(0,1);
                int points = Points.getChallPoints(channel, sender);
                if(r == 0) {
                    Points.removePoints(channel, sender, points);
                    Points.addPoints(channel, whostartedchall, points);
                    irc.SendChatMessage(channel, string.Format("{0} wygrywa pojedynek z {1} PogChamp i wygrywa {2} {3}", whostartedchall, sender, points, channelinfo.pointsConfig.pointsNameMultiple));
                    CancelChallenge(channel, sender);
                } else if(r == 1) {
                    Points.removePoints(channel, whostartedchall, points);  
                    Points.addPoints(channel, sender, points);
                    irc.SendChatMessage(channel, string.Format("{0} wygrywa pojedynek z {1} PogChamp i wygrywa {2} {3}", sender, whostartedchall, points, channelinfo.pointsConfig.pointsNameMultiple));
                    CancelChallenge(channel, sender);
                }
            } else {
                //irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");
            }
        }
        public static void HandleDonate(Message message, IRC irc)
        {
            String channel = message.channel;
            String msg = message.msg;
            String sender = message.sender; 

            string[] m = msg.Split(' ');
            if (m.Length >= 3)
            {
                int points;
                if (Int32.TryParse(m[2], out points) && points > 0) {
                    int sender_points = Points.getPoints(channel, sender);
                    int user_points = Points.getPoints(channel, m[1]);
                    if((sender_points >=points) && (sender_points != -1) && (user_points != -1) ) {
                        Points.removePoints(channel, sender, points);
                        Points.addPoints(channel, m[1], points);
                        Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);
                        message.msg = string.Format("@{0} przekazuje Ci {1} {2} ",sender, points, channelinfo.pointsConfig.pointsNameMultiple);
                        message.inputId = Message.InputId.whisper;
                        message.sender = m[1];
                        irc.SendResponse(message);

                        message.msg = string.Format("Przekazałeś/aś {0} {1} ", points, channelinfo.pointsConfig.pointsNameMultiple, m[1]);
                        message.sender = sender;
                        irc.SendResponse(message);
                    }
                }
            }
        }
        public static void HandleShowPoints(Message message, IRC irc)
        {
            String msg = message.msg;
            String sender = message.sender; 

            string[] m = msg.Split(' ');
            if (m.Length >= 2)
            {
                if (m[1].Equals("all")) {
                    if(m.Length >= 3) {
                        // handle 3rd 
                        ShowTotalPoints(message, m[2], irc);
                        return;
                    } else {
                        ShowTotalPoints(message, sender, irc);
                        return;
                    }
                }
                ShowPoints(message, m[1], irc);
            }
            else if (m.Length == 1)
            {
                ShowPoints(message, sender, irc);
            }
        }

        public static void CancelChallenge(string channel, string name) {
            Points.clearChallenger(channel,"", name, 0);
            Log.WriteLine("Clearing chall from " + name);
        }

        public static void CancelChallengeAfterSeconds(string channel, string name, int timeout) {
            Log.WriteLine("Preparing to clear.. " + name);
            Thread.Sleep(timeout);
            Points.clearChallenger(channel,"", name, 0);
            Log.WriteLine("Clearing chall from " + name);
        }

        private static void StartCancelThread(string channel, string name, int timeout) {
            Thread t = new Thread(() => CancelChallengeAfterSeconds(channel, name, timeout));
            t.IsBackground = true;
            t.Start();
        }
        public static void ShowPoints(Message message, string name, IRC irc)
        {
            string channel = message.channel;
            Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);

            int p = Points.getPoints(channel, name);
            if (p == -1)
            {
                //irc.SendChatMessage(channel, string.Format("Nie ma takiego użytkownika."));
                return;
            }
            string f = string.Format("{0} posiada {1} {2}.", name, p, channelinfo.pointsConfig.pointsNameMultiple);
            irc.SendChatMessage(channel, f);
        }

        public static void ShowTotalPoints(Message message, string name, IRC irc)
        {
            string channel = message.channel;
            Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);
            int p = Points.getTotalPoints(channel, name);
            if (p == -1)
            {
                //irc.SendChatMessage(channel, string.Format("Nie ma takiego użytkownika."));
                return;
            }
            string f = string.Format("{0} posiadał łącznie {1} {2}.", name, p, channelinfo.pointsConfig.pointsNameMultiple);
            irc.SendChatMessage(channel, f);
        }

        public static void HandleRoulette(string channel, string name, string msg, IRC irc)
        {
            Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);
            string[] m = msg.Split(' ');
            if (m.Length >= 2)
            {
                int p;
                if (Int32.TryParse(m[1], out p))
                {
                    if (p <= 0)
                    {
                        //irc.SendChatMessage(channel, string.Format("{0} nie mogą być ujemne.", channelinfo.pointsConfig.pointsNameMultiple));
                    }
                    else
                    {
                        // Handle given value
                        Roulette(channel, name, p, irc);
                    }
                }
                else
                {
                    if (m[1].Contains("%"))
                    {
                        // Handle % value
                        int curPoints = Points.getPoints(channel, name);
                        float prc;
                        int temp;
                        if (Int32.TryParse(m[1].Replace("%", ""), out temp))
                        {
                            prc = temp / 100f;
                            if (prc <= 0.0f)
                            {
                                //irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");
                                return;
                            }
                            curPoints = (int)(prc * (float)curPoints);
                            Roulette(channel, name, curPoints, irc);
                        }
                        else
                        {
                            //irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");
                        }
                    }
                    else if (m[1].Equals("all"))
                    {
                        // Handle all
                        int curPoints = Points.getPoints(channel, name);
                        Roulette(channel, name, curPoints, irc);
                    }
                }
            }
            else
            {
                //irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");

            }
        }
        public static void Roulette(string channel, string name, int points, IRC irc)
        {

            Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);
            if(points > Points.getPoints(channel, name))  {
                irc.SendChatMessage(channel, string.Format("{0} ma za mało {1}", name, channelinfo.pointsConfig.pointsNameMultiple));
                return;
            }
            int r = RandomN.getRandomUnsignedIntFromRange(0, 1);
            if (r == 0)
            {
                // user von
                irc.SendChatMessage(channel, string.Format("{0} wygrywa {1} {2} PogChamp", name, points, channelinfo.pointsConfig.pointsNameMultiple));
                Points.addPoints(channel, name, points);
            }
            else if (r == 1)
            {
                // user lost
                irc.SendChatMessage(channel, string.Format("{0} przegrywa i traci {1} {2} FeelsBadMan", name, points, channelinfo.pointsConfig.pointsNameMultiple));
                Points.removePoints(channel, name, points);

            }
        }
    }
}