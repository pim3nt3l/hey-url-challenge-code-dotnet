using System;

namespace HeyUrlChallengeCodeDotnet.Data
{
    public class UrlClick
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ShortUrlId { get; set; }
        public string Browser { get; set; }
        public string Platform { get; set; }
        public virtual Url Url { get; set; }
    }
}
