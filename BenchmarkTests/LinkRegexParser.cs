using BenchmarkDotNet.Attributes;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BenchmarkTests
{
    [MemoryDiagnoser]
    public class LinkRegexParser
    {
        [Params("www.smokeball.com.au", "www.notfound.au.co", "www.redkeyconveyancing.com.au")]
        public string TargetUrl { get; set; }
        private string HtmlPage { get; set; }
        private Regex Parser { get; set; }

        private const string QueryContainerClass = "kCrYT";
        private readonly string QueryRegexPattern = $"<div class=\"{QueryContainerClass}\">+?<a[^>]+?" +
            $"href=\"\\/url\\?q=(http[\\w\\d:\\/.]+)";

        [GlobalSetup]
        public void Setup()
        {
            HtmlPage = File.ReadAllText("TestPages/_allLinks.html");
            Parser = new Regex(QueryRegexPattern);
        }

        //baseline, checking complete matching then retrieval
        [Benchmark(Baseline = true)]
        public int FindDomainPosition()
        {
            var matches = Parser.Matches(HtmlPage);
            var targetUrl = TargetUrl.ToLower();
            return matches.ToList().FindIndex(x => x.Value.ToLower().Contains(targetUrl));
        }

        [Benchmark]
        public int FindDomainPositionByLoop()
        {
            var matches = Parser.Matches(HtmlPage);
            var index = 0;
            var targetUrl = TargetUrl.ToLower();

            foreach (Match match in matches)
            {
                if (match.Groups[1].Value.ToLower().Contains(targetUrl))
                    return index;
                ++index;
            }

            return -1;
        }

        [Benchmark]
        public int FindDomainPositionMatchByMatch()
        {
            bool containsUrls(string a)
                => a.ToLower().Contains(TargetUrl.ToLower());

            var index = -1;
            var match = Parser.Match(HtmlPage);
            while (match.Success)
            {
                ++index;

                if (match.Groups.Count > 1 && containsUrls(match.Groups[1].Value))
                    return index;
                else
                    match = match.NextMatch();
            }

            return -1;
        }
    }
}
