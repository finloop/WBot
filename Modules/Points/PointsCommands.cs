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
        public static void HandleStartChallenge(string channel, string sender, string msg, IRC irc) {
            // pattern is: !challenge user int(points)
            // m[1] - user 
            // [2] - points
            Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);
            string[] m = msg.Split(" ");
            int points;
            if(m.Length >= 3) {
                if(!Points.isUserChallenged(channel, m[1]) && Int32.TryParse(m[2], out points)) {
                    if(points > 0) {
                        if(points > Points.getPoints(channel, sender)) {
                            irc.SendChatMessage(channel, string.Format("{0} ma za mało {1}", sender, channelinfo.pointsConfig.pointsNameMultiple));
                            return;
                        }
                        if(points > Points.getPoints(channel, m[1])) {
                            irc.SendChatMessage(channel, string.Format("{0} ma za mało {1}", m[1], channelinfo.pointsConfig.pointsNameMultiple));
                            return;
                        }

                        Points.setChallenger(channel, sender, m[1], points);
                        irc.SendChatMessage(channel, string.Format("{0} wyzwał na pojedynek {1} wpisz !{2} aby akceptować pojedynek", sender, m[1], channelinfo.pointsConfig.challengeAccept));
                        StartCancelThread(channel, m[1], 30000);
                        return;
                    }

                } else {
                    irc.SendChatMessage(channel, "Coś poszło nie tak monkaS");
                    return;
                }
            } else {
                irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");
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
                irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");
            }
        }
        public static void HandleShowPoints(string channel, string sender, string msg, IRC irc)
        {
            string[] m = msg.Split(' ');
            if (m.Length >= 2)
            {
                if (m[1].Equals("all")) {
                    if(m.Length >= 3) {
                        // handle 3rd 
                        ShowTotalPoints(channel, m[2], irc);
                        return;
                    } else {
                        ShowTotalPoints(channel, sender, irc);
                        return;
                    }
                }
                ShowPoints(channel, m[1], irc);
            }
            else if (m.Length == 1)
            {
                ShowPoints(channel, sender, irc);
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
        public static void ShowPoints(string channel, string sender, IRC irc)
        {
            Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);
            int p = Points.getPoints(channel, sender);
            if (p == -1)
            {
                irc.SendChatMessage(channel, string.Format("Nie ma takiego użytkownika."));
                return;
            }
            string f = string.Format("{0} posiada {1} {2}.", sender, p, channelinfo.pointsConfig.pointsNameMultiple);
            irc.SendChatMessage(channel, f);
        }

        public static void ShowTotalPoints(string channel, string sender, IRC irc)
        {
            Channel channelinfo = ModuleManager.channels.Find(x => x.Name == channel);
            int p = Points.getTotalPoints(channel, sender);
            if (p == -1)
            {
                irc.SendChatMessage(channel, string.Format("Nie ma takiego użytkownika."));
                return;
            }
            string f = string.Format("{0} posiadał łącznie {1} {2}.", sender, p, channelinfo.pointsConfig.pointsNameMultiple);
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
                        irc.SendChatMessage(channel, string.Format("{0} nie mogą być ujemne.", channelinfo.pointsConfig.pointsNameMultiple));
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
                                irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");
                                return;
                            }
                            curPoints = (int)(prc * (float)curPoints);
                            Roulette(channel, name, curPoints, irc);
                        }
                        else
                        {
                            irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");
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
                irc.SendChatMessage(channel, "Polecenie wpisane niepoprawnie.");

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