using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using SearchScraping.Interfaces;
using SearchScraping.Models;
using SearchScraping.Scrapers;
using SearchScraping.Templates;
using System;
using System.Linq;

namespace BenchmarkTests
{
    [MemoryDiagnoser]
    public class FallbackParserBenchmark
    {
        private ISearchResultCaptureTemplate PrimaryTemplate { get; set; }
        private ISearchResultCaptureTemplate RobotTemplate { get; set; }
        private string HumanHtml { get; set; }
        private string RobotHtml { get; set; }
        private Random Rng { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Rng = new Random(0);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("JsonFiles/RegexTemplates.json")
                .Build();

            PrimaryTemplate = configuration.GetSection("google-au").Get<SearchResultCaptureTemplate>();
            RobotTemplate = configuration.GetSection("google-au-robot").Get<SearchResultCaptureTemplate>();

            HumanHtml = System.IO.File.ReadAllText("TestPages/google-au_allLinks.html");
            RobotHtml = System.IO.File.ReadAllText("TestPages/google-au_allLinks_robot.html");
        }

        [Benchmark(Baseline = true)]
        public SearchResult[] RobotParsing()
        {
            var primary = new SearchResultRegex(RobotTemplate);
            return primary.ParseResults(RobotHtml).ToArray();
        }

        [Benchmark]
        public SearchResult[] AlwaysHuman()
        {
            var primary = new FallbackSearchResultRegex(PrimaryTemplate, RobotTemplate);
            return primary.ParseResults(HumanHtml).ToArray();
        }

        [Benchmark]
        public SearchResult[] AlwaysFallback()
        {
            var primary = new FallbackSearchResultRegex(PrimaryTemplate, RobotTemplate);
            return primary.ParseResults(RobotHtml).ToArray();
        }

        [Benchmark]
        public SearchResult[] Random()
        {
            var primary = new FallbackSearchResultRegex(PrimaryTemplate, RobotTemplate);
            var human = Rng.Next(0, 100) < 50;
            return primary.ParseResults(human ? HumanHtml : RobotHtml).ToArray();
        }
    }
}
