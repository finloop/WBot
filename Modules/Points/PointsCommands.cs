using System;
using System.Collections.Generic;
using Bot.Core;
using Bot.Extensions.MySql;
using Bot.Extensions.Debug;
using System.Threading;
using Bot.Extensions.Stream;

namespace Bot.Modules.Points
{
    public class PointsCommands
    {
        public static void HandleShowPoints(string channel, string sender, string msg, IRC irc) {
            string[] m = msg.Split(' ');
            if(m.Length >= 2) {
                ShowPoints(channel, m[1], irc);
            }else if (m.Length == 1) {
                ShowPoints(channel, sender, irc);
            }
        }
        public static void ShowPoints(string channel, string sender, IRC irc) {       
                int p = Points.getPoints(channel,sender);
                if(p == -1) {
                    irc.SendChatMessage(channel,string.Format("Nie ma takiego użytkownika."));
                    return;
                }
                string f = string.Format("{0} posiada {1} {2}.",sender, p, Points.pointsConfig.getChannelByName(channel).pointsNameMultiple);
                irc.SendChatMessage(channel,f);
        }
    }
}