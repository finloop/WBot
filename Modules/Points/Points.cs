using System;
using System.Collections.Generic;
using Bot.Core;
using Bot.Extensions.MySql;
using Bot.Extensions.Debug;
using System.Threading;
using Bot.Extensions.Stream;

namespace Bot.Modules.Points
{
    public class Points : CommandsModule
    {
        private Thread points_thread;
        public static PointsConfig pointsConfig;
        public Points(List<string> _ActiveChannels, IRC _irc) : base(_ActiveChannels, _irc)
        {
            pointsConfig = FileIO.ReadConfigJson(new PointsConfig());
            points_thread = new Thread(HandlePoints);
            points_thread.IsBackground = true;
            points_thread.Start();
            base.addId("!points");
            base.addId("!config pointsName:");
            base.addId("!config pointsNameMultiple:");
            base.addId("!config challengeName:");
            base.addId("!config challengeAccept:");
        }

        override public void HandleMessage(string channel, string msg, string sender)
        {
            for (int i = 0; i < base.getIds().Count; i++)
            {
                string id = base.getIds()[i];
                
                if (msg.StartsWith(id))
                {
                    if (id.Equals("!config pointsName:"))
                    {
                        int index = pointsConfig.Channels.FindIndex(x => x.Name.Equals(channel));
                        string s = msg.Replace("!config pointsName:","");
                        pointsConfig.Channels[index].pointsName = s;
                    }
                    else if (id.Equals("!config pointsNameMultiple:"))
                    {
                        int index = pointsConfig.Channels.FindIndex(x => x.Name.Equals(channel));
                        string s = msg.Replace("!config pointsNameMultiple:","");
                        pointsConfig.Channels[index].pointsNameMultiple = s;
                    }
                    else if (id.Equals("!config challengeName:"))
                    {
                        int index = pointsConfig.Channels.FindIndex(x => x.Name.Equals(channel));
                        string s = msg.Replace("!config challengeName:","");
                        pointsConfig.Channels[index].challengeName = s;
                    }
                    else if (id.Equals("!config challengeAccept:"))
                    {
                        int index = pointsConfig.Channels.FindIndex(x => x.Name.Equals(channel));
                        string s = msg.Replace("!config challengeAccept:","");
                        pointsConfig.Channels[index].challengeAccept = s;
                    }
                    //CODE
                    for (int k = 0; k < pointsConfig.Channels.Count; k++)
                    {
                        if (id.Equals(pointsConfig.Channels[k].pointsName))
                        {
                            // handle showpoints()
                        }
                        else if (id.Equals(pointsConfig.Channels[k].challengeName))
                        {
                            // handle startChannenge()
                        }
                    }
                    break;
                }
            }
            PointsCommands.Handle(channel,msg,sender,base.irc);
        }

        override public bool AddToChannel(string channel)
        {
            for (int i = 0; i < ActiveChannels.Count; i++)
            {
                if (channel.Equals(ActiveChannels[i]))
                {
                    return false;
                }
            }
            ActiveChannels.Add(channel);

            PointsConfig.Channel ch = new PointsConfig.Channel();
            ch.Name = channel;
            ch.pointsName = "points";
            ch.pointsNameMultiple = "points";
            pointsConfig.Channels.Add(ch);
            string sb = string.Format("use VIEWERS; CREATE TABLE `{0}` (`Name` text COLLATE utf8mb4_unicode_ci,`Points` int(11) DEFAULT NULL,`TotalPoints` int(11) DEFAULT NULL,`Challenger` text COLLATE utf8mb4_unicode_ci) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;", channel);
            MySqlWrapper.MakeQuery(sb);
            FileIO.WriteConfigJson(ch);
            return true;
        }
        public override bool RemoveFromChannel(string channel)
        {
            for (int i = 0; i < ActiveChannels.Count; i++)
            {
                if (channel.Equals(ActiveChannels[i]))
                {
                    ActiveChannels.Remove(channel);
                    pointsConfig.Channels.RemoveAt(pointsConfig.Channels.FindIndex(x => x.Name.Equals(channel)));
                    return true;
                }
            }
            return false;
        }
        static public void AddPointsIfOnChannel(string channel)
        {
            if (true)
            {
                List<string> v = Chatters.GetViewers(channel);
                int n;
                if (v != null)
                {
                    for (int i = 0; i < v.Count; i++)
                    {
                        if (doesUserExist(channel, v[i]))
                        {
                            addPoints(channel, v[i], 1);
                        }
                    }
                }
            }
        }

        public static void addPoints(string channel, string name, int points)
        {
            int _points = getPoints(channel, name);
            int _totalPoints = getTotalPoints(channel, name);
            setPoints(channel, name, _points + points);
            setTotalPoints(channel, name, _totalPoints + points);
        }
        public static void removePoints(string channel, string name, int points)
        {
            int _points = getPoints(channel, name);
            setPoints(channel, name, _points - points);
        }
        public static bool doesUserExist(string channel, string name)
        {

            string fill = string.Format("select Name from VIEWERS.{0} where Name = \"{1}\"", channel, name);
            List<string> query = MySqlWrapper.MakeQuery(fill, "Name");
            if (query.Count > 0)
            {
                string n = query[0];
                if (!name.Equals(""))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {

            }

            return false;
        }
        public static void addUser(string channel, string name)
        {
            string sb = string.Format("insert into VIEWERS.{0} (Name, Points, TotalPoints) values (\"{1}\", 0, 0)", channel, name);
            MySqlWrapper.MakeQuery(sb, "Points");
        }
        public static int getPoints(string channel, string name)
        {
            if (doesUserExist(channel, name))
            {

                string sb = string.Format("select Points from VIEWERS.{0} where Name = \"{1}\"", channel, name);
                List<string> query = MySqlWrapper.MakeQuery(sb, "Points");
                int i;
                Int32.TryParse(query[0], out i);
                return i;
            }
            return -1;
        }
        public static int getTotalPoints(string channel, string name)
        {
            if (doesUserExist(channel, name))
            {
                string sb = string.Format("select TotalPoints from VIEWERS.{0} where Name = \"{1}\"", channel, name);
                List<string> query = MySqlWrapper.MakeQuery(sb, "TotalPoints");
                int i;
                Int32.TryParse(query[0], out i);
                return i;
            }
            return -1;
        }

        public static void setPoints(string channel, string name, int points)
        {
            if (doesUserExist(channel, name))
            {
                string sb = string.Format("UPDATE VIEWERS.{0} SET Points = {1} WHERE Name = \"{2}\"", channel, points, name);
                MySqlWrapper.MakeQuery(sb, "Points");
            }
        }
        public static void setTotalPoints(string channel, string name, int points)
        {
            if (doesUserExist(channel, name))
            {
                string sb = string.Format("UPDATE VIEWERS.{0} SET TotalPoints = {1} WHERE Name = \"{2}\"", channel, points, name);
                MySqlWrapper.MakeQuery(sb, "TotalPoints");
            }
        }
        private void HandlePoints()
        {
            while (true)
            {
                for (int i = 0; i < getActiveChannels().Count; i++)
                    AddPointsIfOnChannel(getActiveChannels()[i]);
                Thread.Sleep(30000);
            }
        }
    }
}