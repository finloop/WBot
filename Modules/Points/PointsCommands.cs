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
        public static void HandleStartHallenge(string channel, string sender, string msg, IRC irc) {
            // pattern is: !challenge user int(points)
            // m[1] - user 
            // [2] - points
            string[] m = msg.Split(" ");
            int points;
            if(m.Length >= 3) {
                if(!Points.isUserChallenged(channel, m[1]) && Int32.TryParse(m[2], out points)) {
                    if(points > 0) {
                        Points.setChallenger(channel, sender, m[1], points);
                        irc.SendChatMessage(channel, string.Format("{0} wyzwał na pojedynek {1} wpisz !{2} aby akceptować pojedynek", sender, m[1], Points.pointsConfig.getChannelByName(channel).challengeAccept));
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
        public static void ShowPoints(string channel, string sender, IRC irc)
        {
            int p = Points.getPoints(channel, sender);
            if (p == -1)
            {
                irc.SendChatMessage(channel, string.Format("Nie ma takiego użytkownika."));
                return;
            }
            string f = string.Format("{0} posiada {1} {2}.", sender, p, Points.pointsConfig.getChannelByName(channel).pointsNameMultiple);
            irc.SendChatMessage(channel, f);
        }

        public static void ShowTotalPoints(string channel, string sender, IRC irc)
        {
            int p = Points.getTotalPoints(channel, sender);
            if (p == -1)
            {
                irc.SendChatMessage(channel, string.Format("Nie ma takiego użytkownika."));
                return;
            }
            string f = string.Format("{0} posiadał łącznie {1} {2}.", sender, p, Points.pointsConfig.getChannelByName(channel).pointsNameMultiple);
            irc.SendChatMessage(channel, f);
        }

        public static void HandleRoulette(string channel, string name, string msg, IRC irc)
        {
            string[] m = msg.Split(' ');
            if (m.Length >= 2)
            {
                int p;
                if (Int32.TryParse(m[1], out p))
                {
                    if (p <= 0)
                    {
                        irc.SendChatMessage(channel, string.Format("{0} nie mogą być ujemne.", Points.pointsConfig.getChannelByName(channel).pointsName));
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
            int r = RandomN.getRandomUnsignedIntFromRange(0, 1);
            if (r == 0)
            {
                // user von
                irc.SendChatMessage(channel, string.Format("{0} wygrywa {1} {2} PogChamp", name, points, Points.pointsConfig.getChannelByName(channel).pointsNameMultiple));
                Points.addPoints(channel, name, points);
            }
            else if (r == 1)
            {
                // user lost
                irc.SendChatMessage(channel, string.Format("{0} przegrywa i traci {1} {2} FeelsBadMan", name, points, Points.pointsConfig.getChannelByName(channel).pointsNameMultiple));
                Points.removePoints(channel, name, points);

            }
        }
    }
}