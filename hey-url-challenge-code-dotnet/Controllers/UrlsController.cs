using System;
using System.Collections.Generic;
using HeyUrlChallengeCodeDotnet.Models;
using HeyUrlChallengeCodeDotnet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shyjus.BrowserDetection;

namespace HeyUrlChallengeCodeDotnet.Controllers
{
    [Route("/")]
    public class UrlsController : Controller
    {
        private readonly ILogger<UrlsController> _logger;
        private readonly IShortUrlService _shortUrlService;
        private static readonly Random getrandom = new Random();
        private readonly IBrowserDetector browserDetector;

        public UrlsController(ILogger<UrlsController> logger, IBrowserDetector browserDetector, IShortUrlService shortUrlService)
        {
            this.browserDetector = browserDetector;
            _logger = logger;
            _shortUrlService = shortUrlService;
        }
        public IActionResult Index()
        {
            var model = new HomeViewModel();
            model.Urls = this._shortUrlService.GetAll();
            model.NewUrl = new();
            return View(model);
        }

        [Route("/{url}")]
        public IActionResult Visit(string url)
        {
            var model = this._shortUrlService.Get(url);
            if (model == null) return NotFound();
            this._shortUrlService.AddClickInfo(model.Id, this.browserDetector.Browser.OS, this.browserDetector.Browser.Name);
            return Redirect(model.OriginalUrl);
        }

        [Route("urls/{url}")]
        public IActionResult Show(string url)
        {
            var model = this._shortUrlService.GetMetrics(url);
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(string newUrl)
        {

            try
            {
                this._shortUrlService.Create(newUrl);
            }
            catch (Exception e)
            {
                TempData["Notice"] = e.Message;
            }
            return RedirectToAction("Index");
        }
    }
}