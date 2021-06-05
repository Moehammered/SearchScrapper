using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BenchmarkTests
{
    [MemoryDiagnoser]
    public class RegexComparisonBenchmark
    {
        [Params(
            "Link+Heading Capture",
            "Partial Link Capture",
            "Full Link Capture"
            )]
        public string RegexPattern { get; set; }

        private readonly Dictionary<string, Regex> Patterns = new Dictionary<string, Regex>
        {
            {
                "Link+Heading Capture",
                new Regex("<div class=\"kCrYT\">+?<a[^>]+?href=\"\\/url\\?q=(http[s]?\\:.+?)\">.+?div.+?>(.+?)<\\/div><\\/h\\d?>")
            },
            {
                "Partial Link Capture",
                new Regex("<div class=\"kCrYT\">+?<a[^>]+?href=\"\\/url\\?q=(http[s]?\\:.+?)\">.+?div.+?>(.+?)<\\/div><\\/h\\d?>")
            },
            {
                "Full Link Capture",
                new Regex("<div class=\"kCrYT\">+?<a[^>]+?href=\"\\/url\\?q=(http[s]?\\:.+?)\">.+?div.+?>(.+?)<\\/div><\\/h\\d?>")
            }
        };

        private string HtmlPage { get; set; }
        [Params("www.smokeball.com.au", "www.notfound.au.co", "www.redkeyconveyancing.com.au")]
        public string TargetUrl { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            HtmlPage = File.ReadAllText("TestPages/_allLinks.html");
        }

        //baseline, checking complete matching then retrieval
        [Benchmark(Baseline = true)]
        public int FindDomainPosition()
        {
            var matches = Patterns[RegexPattern].Matches(HtmlPage);
            var targetUrl = TargetUrl.ToLower();
            return matches.ToList().FindIndex(x => x.Value.ToLower().Contains(targetUrl));
        }

        [Benchmark]
        public int FindDomainPositionByLoop()
        {
            var matches = Patterns[RegexPattern].Matches(HtmlPage);
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
            var match = Patterns[RegexPattern].Match(HtmlPage);
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
