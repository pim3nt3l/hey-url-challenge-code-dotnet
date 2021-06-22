using System;
using System.Collections.Generic;

namespace HeyUrlChallengeCodeDotnet.Data
{
    public class Url
    {
        public Guid Id { get; set; }
        public string ShortUrl { get; set; }
        public string OriginalUrl { get; set; }
        public int Count { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<UrlClick> Clicks { get; set; }
        public Url()
        {
            this.Clicks = new HashSet<UrlClick>();
        }
    }
}
