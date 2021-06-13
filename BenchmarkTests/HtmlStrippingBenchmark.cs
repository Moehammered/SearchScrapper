using BenchmarkDotNet.Attributes;
using CommandLine;
using SearchScraping.Scrapers;
using SearchScraping.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkTests
{
    [MemoryDiagnoser]
    public class HtmlStrippingBenchmark
    {
        private HtmlToXmlParser Parser { get; set; }
        private HtmlSanitiser Sanitiser { get; set; }

        private string HumanHtml { get; set; }
        private string RobotHtml { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Parser = new HtmlToXmlParser();
            Sanitiser = new HtmlSanitiser();
            HumanHtml = System.IO.File.ReadAllText("TestPages/google-au_allLinks.html");
            RobotHtml = System.IO.File.ReadAllText("TestPages/google-au_allLinks_robot.html");
        }

        [Benchmark(Baseline = true)]
        public string CleanupRobotHtmlViaSubStr()
        {
            return Parser.StripHtmlTest(RobotHtml);
        }

        [Benchmark]
        public string CleanupRobotHtmlViaSpan()
        {
            return Parser.StripHtmlTestViaSpan(RobotHtml).ToString();
        }

        [Benchmark]
        public string CleanupHumanHtmlViaSubStr()
        {
            return Parser.StripHtmlTest(HumanHtml);
        }

        [Benchmark]
        public string CleanupHumanHtmlViaSpan()
        {
            return Parser.StripHtmlTestViaSpan(HumanHtml).ToString();
        }

        [Benchmark]
        public string SanitiseHumanHtmlStrBuild()
        {
            return Sanitiser.Sanitise(HumanHtml);
        }

        [Benchmark]
        public string SanitiseRobotHtmlStrBuild()
        {
            return Sanitiser.Sanitise(RobotHtml);
        }
    }
}
