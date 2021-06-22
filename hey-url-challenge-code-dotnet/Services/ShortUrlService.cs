using HeyUrlChallengeCodeDotnet.Models;
using HeyUrlChallengeCodeDotnet.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace HeyUrlChallengeCodeDotnet.Services
{
    public interface IShortUrlService
    {
        string Create(string url);
        Url Get(string shortUrl);

        void Update(UrlViewModel model);
        void Update(string shortUrl);
        ShowViewModel GetMetrics(Guid id);
        ShowViewModel GetMetrics(string url);

        IList<UrlViewModel> GetAll();
        void AddClickInfo(Guid shortUrlId, string platform, string browser);
    }

    public class ShortUrlService : IShortUrlService
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<ShortUrlService> _logger;

        public ShortUrlService(ApplicationContext dbcontext, ILogger<ShortUrlService> logger)
        {
            this._context = dbcontext;
            this._logger = logger;
        }

        public string Create(string urlToShort)
        {
            //Call extension method to get a short URL
            var shortedUrl = this.ToShortUrl(urlToShort);
            //Check if the shorten url exists. Shouldn't exists
            if (this._context.Urls.Any(x => x.ShortUrl == shortedUrl))
            {
                throw new InvalidOperationException($"An shorted url: {urlToShort} already exists");

            }
            var newUrl = new Url()
            {
                Count = 0,
                Id = Guid.NewGuid(),
                OriginalUrl = urlToShort,
                ShortUrl = shortedUrl,
                CreatedAt = DateTime.Now
            };
            this._context.Add(newUrl);
            this._context.SaveChanges();
            return shortedUrl;
        }

        public Url Get(string shortUrl)
        {
            return this._context.Urls.FirstOrDefault(m => m.ShortUrl == shortUrl);
        }

        public Url Get(Guid id)
        {
            return this._context.Urls.Find(id);
        }

        /// <summary>
        /// Create a url identifier using a hash
        /// </summary>
        /// <param name="urlToShort"></param>
        /// <returns></returns>
        public string ToShortUrl(string urlToShort)
        {
            //It MUST have 5 characters in length e.g.NELNT
            //It MUST generate only upper case letters
            //It MUST NOT generate special characters
            //It MUST NOT generate whitespace
            //It MUST be unique
            string outputHash = "";
            using (var md5 = new MD5CryptoServiceProvider())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(urlToShort);
                buffer = md5.ComputeHash(buffer);
                outputHash = buffer.Aggregate("", (current, cByte) => current += cByte.ToString("x2"));
            }
            var regex = new Regex(@"\d");
            return regex.Replace(outputHash, "").Substring(0, 5).ToUpper();
        }

        public void Update(UrlViewModel model)
        {
            //lookup by shortenUrl id
            var url = this.Get(model.Id);
            url.Count++; //Increment the clicks
            this._context.Update(url); //Update changetracker
            this._context.SaveChanges(); //Save change to database store
        }

        public void Update(string shortUrl)
        {
            //lookup by shortenUrl
            var url = this.Get(shortUrl);
            url.Count++; //Increment the clicks
            this._context.Update(url); //Update changetracker
            this._context.SaveChanges(); //Save change to database store
        }

        public ShowViewModel GetMetrics(Guid id)
        {
            //lookup by shortenUrl id
            var urlModel = this.Get(id);
            return this.ConvertToMetricsModel(urlModel);
        }
        public ShowViewModel GetMetrics(string shortUtl)
        {
            //lookup by shortenUrl id
            var urlModel = this.Get(shortUtl);
            return this.ConvertToMetricsModel(urlModel);
        }

        public ShowViewModel ConvertToMetricsModel(Url urlModel)
        {
            var metrics = urlModel.Clicks;
            return new ShowViewModel()
            {
                Url = new UrlViewModel
                {
                    ShortUrl = urlModel.ShortUrl,
                    Count = urlModel.Count,
                    Id = urlModel.Id,
                    CreatedAt = urlModel.CreatedAt,
                    OriginalUrl = urlModel.OriginalUrl
                },
                PlatformClicks = urlModel.Clicks.GroupBy(t => t.Platform).ToDictionary(t => t.Key, t => metrics.Count(t => t.Platform == t.Platform)),
                DailyClicks = urlModel.Clicks.GroupBy(t => t.CreatedAt.Date).ToDictionary(t => t.Key.DayOfWeek.ToString(), t => metrics.Count(t => t.CreatedAt.Date == t.CreatedAt.Date)),
                BrowseClicks = urlModel.Clicks.GroupBy(t => t.Browser).ToDictionary(t => t.Key, t => metrics.Count(t => t.Platform == t.Platform))
            };
        }


        public IList<UrlViewModel> GetAll()
        {
            return this._context.Urls.Select(t => new UrlViewModel()
            {
                Count = t.Count,
                Id = t.Id,
                OriginalUrl = t.OriginalUrl,
                ShortUrl = t.ShortUrl,
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        public void AddClickInfo(Guid shortUrlId, string platform, string browser)
        {
            var url = this.Get(shortUrlId);
            url.Count++;
            var newClick = new UrlClick()
            {
                Browser = browser,
                CreatedAt = DateTime.Now,
                Platform = platform,
                ShortUrlId = shortUrlId
            };
            this._context.Add(newClick);
            this._context.Update(url);
            this._context.SaveChanges();
        }
    }
}
