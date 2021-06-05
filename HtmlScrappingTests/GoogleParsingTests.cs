using HtmlScrappingTests.DemoModels;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlScrappingTests
{
    [TestClass]
    public class GoogleParsingTests
    {
        private const string EarlyUrl = "www.smokeball.com.au";
        private const string MidwayUrl = "www.redkeyconveyancing.com.au";
        private const string LateUrl = "www.onlineconveyancingcentre.com.au";
        private const string NotFoundUrl = "www.smokbal.com.au";

        [TestMethod]
        public void TestConfig()
        {
            var googleEngine = LoadGoogleData();
            Assert.IsNotNull(googleEngine);
        }

        [TestMethod]
        public void TestRegexMatching()
        {
            var googleEngine = LoadGoogleData();
            var searchResponse = LoadMonolithHtml();

            var regex = new Regex(googleEngine.BuildRegexPattern());
            Assert.IsTrue(regex.IsMatch(searchResponse));
        }

        [TestMethod]
        public void ExtractLinkData()
        {
            var googleEngine = LoadGoogleData();
            var searchResponse = LoadMonolithHtml();

            var regex = new Regex(googleEngine.BuildRegexPattern());
            var matches = regex.Matches(searchResponse);

            Assert.IsTrue(matches.Count == 100);
        }

        [TestMethod]
        public void FindEarlyLinkPosition()
        {
            var position = FindUrlPosition(EarlyUrl);
            Assert.AreEqual(position, 2);
        }

        [TestMethod]
        public void FindHalfwayLinkPosition()
        {
            var position = FindUrlPosition(MidwayUrl);
            Assert.AreEqual(position, 57);
        }

        [TestMethod]
        public void FindLateLinkPosition()
        {
            var position = FindUrlPosition(LateUrl);
            Assert.AreEqual(position, 91);
        }

        [TestMethod]
        public void FindNoLinkPosition()
        {
            var position = FindUrlPosition(NotFoundUrl);
            Assert.AreEqual(position, -1);
        }

        [TestMethod]
        public void ParseAllResults()
        {
            var results = ParseResults(LoadMonolithHtml());

            Assert.IsTrue(results.Any());

            Assert.IsTrue(results.Any(x => x.Url.Contains(EarlyUrl)));
            Assert.IsTrue(results.Any(x => x.Url.Contains(MidwayUrl)));
            Assert.IsTrue(results.Any(x => x.Url.Contains(LateUrl)));
            Assert.IsFalse(results.Any(x => x.Url.Contains(NotFoundUrl)));
        }

        [TestMethod]
        public void FindEarlyResult()
        {
            var results = ParseResults(LoadMonolithHtml());

            var earlyResult = results.First(x => x.Url.ToLower().Contains(EarlyUrl.ToLower()));
            Assert.AreEqual(earlyResult.Heading, "Best Conveyancing Matter Management Software - Smokeball");
            Assert.AreEqual(earlyResult.Position, 2);
        }

        [TestMethod]
        public void FindHalfwayResult()
        {
            var results = ParseResults(LoadMonolithHtml());

            var midwayResult = results.First(x => x.Url.ToLower().Contains(MidwayUrl.ToLower()));
            Assert.AreEqual(midwayResult.Heading, "Conveyancing Coffs Harbour | Fixed Fee Conveyancing Toormina ...");
            Assert.AreEqual(midwayResult.Position, 57);
        }

        [TestMethod]
        public void FindLateResult()
        {
            var results = ParseResults(LoadMonolithHtml());

            var lateResult = results.First(x => x.Url.ToLower().Contains(LateUrl.ToLower()));
            Assert.AreEqual(lateResult.Heading, "About us - Online Conveyancing Centre");
            Assert.AreEqual(lateResult.Position, 91);
        }

        [TestMethod]
        public void FindNoResult()
        {
            var results = ParseResults(LoadMonolithHtml());
            var action = new Action(() => results.First(x => x.Url.ToLower().Contains(NotFoundUrl.ToLower())));
            Assert.ThrowsException<InvalidOperationException>(action);
        }

        private IEnumerable<SearchResult> ParseResults(string query)
        {
            var googleEngine = LoadGoogleData();
            var regex = new Regex(googleEngine.BuildRegexPattern());

            var index = 0;
            var match = regex.Match(query);
            while (match.Success)
            {
                ++index;

                if (match.Groups.Count > 1)
                    yield return new SearchResult() 
                    {
                        Position = index,
                        Url = match.Groups[1].Value,
                        Heading = match.Groups[2].Value
                    };

                match = match.NextMatch();
            }
        }

        private int FindUrlPosition(string url)
        {
            bool containsUrl(string link)
                => link.ToLower().Contains(url);

            var googleEngine = LoadGoogleData();
            var searchResponse = LoadMonolithHtml();

            var regex = new Regex(googleEngine.BuildRegexPattern());

            var index = 0;
            var match = regex.Match(searchResponse);
            while (match.Success)
            {
                ++index;

                if (match.Groups.Count > 1 && containsUrl(match.Groups[1].Value))
                    return index;
                else
                    match = match.NextMatch();
            }

            return -1;
        }

        private SearchEngineConfiguration LoadGoogleData()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("SearchEngineConfiguration.json")
                .Build();

            var searchEngines = config.GetSection("searchEngines")
                .Get<IEnumerable<SearchEngineConfiguration>>();

            return searchEngines.First(x => x.Name.Equals("google-au", System.StringComparison.OrdinalIgnoreCase));
        }

        private string LoadMonolithHtml()
            => File.ReadAllText("google-parsing-tests/google-au_allLinks.html");
    }
}
