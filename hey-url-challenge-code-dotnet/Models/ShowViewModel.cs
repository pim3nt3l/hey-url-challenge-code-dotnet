using System.Collections.Generic;
using HeyUrlChallengeCodeDotnet.Models;

namespace HeyUrlChallengeCodeDotnet.Models
{
    public class ShowViewModel
    {
        public UrlViewModel Url { get; set; }
        public Dictionary<string, int> DailyClicks { get; set; }
        public Dictionary<string, int> BrowseClicks { get; set; }
        public Dictionary<string, int> PlatformClicks { get; set; }
    }
}