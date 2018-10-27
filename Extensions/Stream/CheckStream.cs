using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bot.Core;

namespace Bot.Extensions.Stream
{
    public class CheckStream
    {
        static private JsonStream json = null;

        static private void read(string channel)
        {
            if (Chatters.config == null)
                Chatters.config = FileIO.ReadConfigParameters("Config.json");
            else
            {
                String text;
                WebClient web = new WebClient();
                System.IO.Stream stream = web.OpenRead("https://api.twitch.tv/kraken/streams/" + channel + "?client_id=" + Chatters.config.clientID);
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                    json = JsonConvert.DeserializeObject<JsonStream>(text);
                }
            }
        }
        static public bool isRunning(string channel)
        {
            read(channel);
            if(json != null)
            if (json.stream == null)
                return false;
            else
                return true;
            return false;
        }

        static public TimeSpan Uptime(string channel)
        {
            if (isRunning(channel))
            {
                TimeSpan dateTime = DateTime.UtcNow - json.stream.created_at.ToUniversalTime();
                return dateTime;
            }
            else
            {
                return TimeSpan.Zero;
            }
        }

        public class Preview
        {
            public string small { get; set; } = null;
            public string medium { get; set; } = null;
            public string large { get; set; } = null;
            public string template { get; set; } = null;
        }

        public class Links
        {
            public string self { get; set; } = null;
            public string follows { get; set; } = null;
            public string commercial { get; set; } = null;
            public string stream_key { get; set; } = null;
            public string chat { get; set; } = null;
            public string features { get; set; } = null;
            public string subscriptions { get; set; } = null;
            public string editors { get; set; } = null;
            public string teams { get; set; } = null;
            public string videos { get; set; } = null;
        }

        public class Channel
        {
            public bool mature { get; set; } = false;
            public bool partner { get; set; } = false;
            public string status { get; set; } = null;
            public string broadcaster_language { get; set; } = null;
            public string display_name { get; set; } = null;
            public string game { get; set; } = null;
            public string language { get; set; } = null;
            public int _id { get; set; } = 0;
            public string name { get; set; } = null;
            public DateTime created_at { get; set; } = DateTime.Now;
            public DateTime updated_at { get; set; } = DateTime.Now;
            public object delay { get; set; } = null;
            public string logo { get; set; } = null;
            public object banner { get; set; } = null;
            public string video_banner { get; set; } = null;
            public object background { get; set; } = null;
            public string profile_banner { get; set; } = null;
            public string profile_banner_background_color { get; set; } = null;
            public string url { get; set; } = null;
            public int views { get; set; } = 0;
            public int followers { get; set; } = 0;
            public Links _links { get; set; } = null;
        }

        public class Links2
        {
            public string self { get; set; } = null;
        }

        public class Stream
        {
            public long _id { get; set; } = 0;

            public string game { get; set; } = null;
            public int viewers { get; set; } = 0;
            public int video_height { get; set; } = 0;
            public double average_fps { get; set; } = 0;
            public int delay { get; set; } = 0;
            public DateTime created_at { get; set; } = DateTime.Today;
            public bool is_playlist { get; set; } = false;
            public string stream_type { get; set; } = null;
            public Preview preview { get; set; } = null;
            public Channel channel { get; set; } = null;
            public Links2 _links { get; set; } = null;
        }

        public class Links3
        {
            public string self { get; set; } = null;
            public string channel { get; set; } = null;
        }

        public class JsonStream
        {
            public Stream stream { get; set; } = null;
            public Links3 _links { get; set; } = null;
        }
    }
}