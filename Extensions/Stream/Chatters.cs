using System;
using System.Collections.Generic;
using Bot.Core;
using Bot.Extensions.MySql;
using Bot.Extensions.Debug;
using System.Threading;
using Bot.Extensions.Stream;
using System.Linq;
using System.Net;
using Newtonsoft.Json;


namespace Bot.Extensions.Stream
{
    public class Chatters
    {
        static private JsonChatters json = null;
        static private FileIO.ConfigParameters config;

        static private void read(string channel)
        {
            try
            {
                if (config == null)
                    config = FileIO.ReadConfigParameters("Config.json");
                else
                {
                    String text;
                    MyWebClient web = new MyWebClient();

                    System.IO.Stream stream = web.OpenRead("https://tmi.twitch.tv/group/user/" + channel + "/chatters?client_id=" + config.clientID);
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        text = reader.ReadToEnd();
                        json = JsonConvert.DeserializeObject<JsonChatters>(text);
                    }
                }
            }
            catch (Exception w)
            {
                Debug.Log.Exception(w);
            }
        }

        static public List<String> GetViewers(string channel)
        {
            read(channel);
            if (json != null)
            {
                List<String> k = json.chatters.viewers;
                List<String> l = json.chatters.moderators;
                k.AddRange(l);
                return k;
            }
            else
                return null;
        }


        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 60 * 1000;

                return w;
            }
        }

        private class JsonChatters
        {
            public Links _links { get; set; }
            public int chatter_count { get; set; }
            public Chatters chatters { get; set; }

            public class Links
            {
            }

            public class Chatters
            {
                public List<string> moderators { get; set; }
                public List<string> staff { get; set; }
                public List<string> admins { get; set; }
                public List<string> global_mods { get; set; }
                public List<string> viewers { get; set; }
            }
        }
    }
}