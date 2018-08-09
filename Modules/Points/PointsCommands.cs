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
        public static void Handle(string channel, string msg, string sender, IRC irc) {
            if(msg.ToLower().StartsWith("!points")) {
                
                int p = Points.getPoints(channel,sender);
                string f = string.Format("{0} posiada {1} {2}",channel, p, Points.pointsConfig.getChannelByName(channel).pointsName);
                irc.SendChatMessage(channel,f);
            }
        }
    }
}