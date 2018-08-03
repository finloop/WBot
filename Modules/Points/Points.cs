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


        public Points(List<string> _ActiveChannels, IRC _irc) : base(_ActiveChannels, _irc)
        {
            points_thread = new Thread(HandlePoints);
            points_thread.IsBackground = true;
            points_thread.Start();
        }

        override public void HandleMessage(string channel, string msg, string sender)
        {

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