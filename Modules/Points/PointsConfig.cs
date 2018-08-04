using System.Collections.Generic;

namespace Bot.Modules.Points
{
    public class PointsConfig
    {
        
        public List<Channel> Channels { get; set; }
        public class Channel
        {
            public string Name { get; set; }
            public string pointsName { get; set; }
            public string pointsNameMultiple { get; set; }
            public string challengeName { get; set; }
            public string challengeAccept { get; set; }
        }
    }
}