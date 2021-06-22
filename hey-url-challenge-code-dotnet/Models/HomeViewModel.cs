using System.Collections.Generic;
using HeyUrlChallengeCodeDotnet.Models;

namespace HeyUrlChallengeCodeDotnet.Models
{
    public class HomeViewModel
    {
        public IEnumerable<UrlViewModel> Urls { get; set; }
        public UrlViewModel NewUrl { get; set; }
    }
}
