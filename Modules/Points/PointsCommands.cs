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
        public static void ShowPoints(string channel, string sender, IRC irc) {       
                int p = Points.getPoints(channel,sender);
                string f = string.Format("{0} posiada {1} {2}",sender, p, Points.pointsConfig.getChannelByName(channel).pointsName);
                irc.SendChatMessage(channel,f);
        }
    }
}