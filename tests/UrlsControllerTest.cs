using HeyUrlChallengeCodeDotnet.Controllers;
using HeyUrlChallengeCodeDotnet.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shyjus.BrowserDetection;
using NSubstitute;
using System.Collections.Generic;
using HeyUrlChallengeCodeDotnet.Models;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System.Linq;
using System;

namespace tests
{
    public class UrlsControllerTest
    {
        private ILogger<UrlsController> logger;
        private IShortUrlService shortUrlService;
        private IBrowserDetector browserDetector;
        [SetUp]
        public void Setup()
        {
            logger = Substitute.For<ILogger<UrlsController>>();
            shortUrlService = Substitute.For<IShortUrlService>();
            browserDetector = Substitute.For<IBrowserDetector>();
        }

        [Test]
        public void Index_ShouldReturnsListOfUrl()
        {
            //arrange
            var controller = new UrlsController(logger, browserDetector, shortUrlService);

            //act
            var listUrl = Builder<UrlViewModel>.CreateListOfSize(10) //Create a mock list to return
                .All()
                    .With(x => x.CreatedAt = System.DateTime.Today)
                    .With(x => x.OriginalUrl = "/")
                    .With(x => x.ShortUrl = "ABCDE")
                .Build();
            shortUrlService.GetAll().Returns(listUrl); //Setup service
            //assert
            var response = controller.Index() as ViewResult; //Execute action
            response.Model.Should().NotBeNull(); //Asset model is not null
            var responseModel = response.Model as HomeViewModel;
            responseModel.Urls.Count().Should().Be(listUrl.Count); //Assert model is a list expected
        }

        [Test]
        public void Index_ShouldReturnsAnEmptyList()
        {
            //arrange
            var controller = new UrlsController(logger, browserDetector, shortUrlService);

            //act
            var listUrl = new List<UrlViewModel>();
            shortUrlService.GetAll().Returns(listUrl); //Setup service

            //assert
            var response = controller.Index() as ViewResult; //Execute action
            response.Model.Should().NotBeNull(); //Asset model is not null
            var responseModel = response.Model as HomeViewModel;
            responseModel.Urls.Count().Should().Be(listUrl.Count); //Assert model is a list expected
        }

        [Test]
        public void Visit_ShouldRedirectValidUrl()
        {
            //arrange
            var controller = new UrlsController(logger, browserDetector, shortUrlService);
            var urlToCheck = "ABCDE";
            var originalUrl = "/test";

            //act
            shortUrlService.Get(urlToCheck).Returns(new HeyUrlChallengeCodeDotnet.Data.Url()
            {
                Count = 1,
                CreatedAt = DateTime.Today,
                Id = Guid.Empty,
                OriginalUrl = originalUrl,
                ShortUrl = urlToCheck
            }); //Setup service

            //assert
            var response = controller.Visit(urlToCheck) as RedirectResult; //Execute action
            response.Url.Should().Be(originalUrl); //Asset model is not null
        }


    }
}