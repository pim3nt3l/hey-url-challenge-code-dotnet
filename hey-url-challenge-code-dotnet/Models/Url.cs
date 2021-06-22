using System;

namespace HeyUrlChallengeCodeDotnet.Models
{
    public class UrlViewModel
    {
        public Guid Id { get; set; }
        public string ShortUrl { get; set; }
        public string OriginalUrl { get; set; }
        public int Count { get; set; }
        public DateTime CreatedAt { get; internal set; }
    }
}
