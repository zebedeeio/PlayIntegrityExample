using System.Collections;
using System.Collections.Generic;
namespace ZBD
{
    public class Models
    {
        public class AppUsageStats
        {

            public string packageName { get; set; }
            public long firstTimeStamp { get; set; }
            public long lastTimeStamp { get; set; }
            public long totalTimeInForeground { get; set; }

        }

        public class RewardsResponse
        {
            public long currentTimePlayed;
            public long currentSats;
            public long currentRequiredTime;
            public bool validated;
            public bool whitelisted;
            public bool blacklisted;
            public bool error;
            public string message;
        }

        public class SendToUsernameResponse
        {
            public bool success;
            public string message;
            public long currentTimePlayed;
            public long currentSats;
            public long currentRequiredTime;
        }


        public class PlayIntegrityResponse
        {
            public bool success;
            public string message;
            public string token;
        }


        public class GameData
        {

            public AppUsageStats appUsageStats { get; set; }
            public int touchCounts { get; set; }
            public int accelerometerCount { get; set; }
            public string userId { get; set; }

        }
    }
}