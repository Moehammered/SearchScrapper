using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BenchmarkTests
{
    [MemoryDiagnoser]
    public class QueryParserRegex
    {
        private string HtmlSample { get; set; }
        private string[] HtmlSamples { get; set; }
        private Regex RegexInstance { get; set; }
        [Params("www.smokeball.com.au", "www.notgonnabefound.worstcase.com.au")]
        public string UrlTarget { get; set; }
        private const string QueryResultClass = "kCrYT";
        private const string QueryResultPattern = "<div class=\"{_queryClass_}\">(?:<)a[^>]*href=\"(.*?)\">.*?<.*?<\\/a>";

        [GlobalSetup]
        public void Setup()
        {
            HtmlSample = File.ReadAllText("TestPages/_allLinks.html");
            HtmlSamples = Enumerable.Range(0, 10)
                .Select(x => File.ReadAllText($"TestPages/{x}.html"))
                .ToArray();
            RegexInstance = new Regex(QueryResultPattern.Replace("{_queryClass_}", QueryResultClass), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public IEnumerable<string> FindLinks(string html, string queryClass)
        {
            var matches = Regex.Matches(html, QueryResultPattern.Replace("{_queryClass_}", queryClass), RegexOptions.IgnoreCase);
            return matches.Cast<Match>().Where(x => x.Success).Select(x => x.Groups[1].Value);
        }

        public IEnumerable<string> FindLinksPrepped(string html)
        {
            var matches = RegexInstance.Matches(html);
            return matches.Cast<Match>().Where(x => x.Success).Select(x => x.Groups[1].Value);
        }

        [Benchmark(Baseline = true, Description = "Regex monolith html response")]
        public string[] BenchmarkFindLinks()
            => FindLinks(HtmlSample, QueryResultClass).ToArray();


        [Benchmark()]
        public int BenchmarkFindPosition()
        {
            var links = FindLinks(HtmlSample, QueryResultClass);
            return links.ToList().FindIndex(x => x.ToLower().Contains(UrlTarget));
        }

        [Benchmark]
        public string[] BenchmarkFindSegmentedLinks()
            => HtmlSamples.SelectMany(x => FindLinks(x, QueryResultClass)).ToArray();

        [Benchmark]
        public int BenchmarkFindPositionInSegments()
        {
            var links = HtmlSamples.SelectMany(x => FindLinks(x, QueryResultClass)).ToList();
            return links.FindIndex(x => x.ToLower().Contains(UrlTarget));
        }

        [Benchmark]
        public int BenchmarkFindPositionInSegmentsEarlyOut()
        {
            var links = HtmlSamples.SelectMany(x => FindLinks(x, QueryResultClass));
            var index = -1;
            foreach (var link in links)
            {
                ++index;
                if (link.ToLower().Contains(UrlTarget))
                    return index;
            }

            return index;
        }

        //

        [Benchmark]
        public string[] BenchmarkFindLinksPrepped()
            => FindLinksPrepped(HtmlSample).ToArray();

        [Benchmark]
        public int BenchmarkFindPositionPrepped()
        {
            var links = FindLinksPrepped(HtmlSample);
            return links.ToList().FindIndex(x => x.ToLower().Contains(UrlTarget));
        }

        [Benchmark]
        public string[] BenchmarkFindSegmentedLinksPrepped()
            => HtmlSamples.SelectMany(x => FindLinksPrepped(x)).ToArray();

        [Benchmark]
        public int BenchmarkFindPositionInSegmentsPrepped()
        {
            var links = HtmlSamples.SelectMany(x => FindLinksPrepped(x)).ToList();
            return links.FindIndex(x => x.ToLower().Contains(UrlTarget));
        }

        [Benchmark]
        public int BenchmarkFindPositionInSegmentsEarlyOutPrepped()
        {
            var links = HtmlSamples.SelectMany(x => FindLinksPrepped(x));
            var index = -1;
            foreach (var link in links)
            {
                ++index;
                if (link.ToLower().Contains(UrlTarget))
                    return index;
            }

            return index;
        }
    }
}
