using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using SearchScraping.Interfaces;
using SearchScraping.Models;
using SearchScraping.Scrapers;
using SearchScraping.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BenchmarkTests
{
    [MemoryDiagnoser]
    public class HtmlToXmlBenchmark
    {
        private string HumanHtml { get; set; }
        private string RobotHtml { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            HumanHtml = System.IO.File.ReadAllText("TestPages/google-au_allLinks.html");
            RobotHtml = System.IO.File.ReadAllText("TestPages/google-au_allLinks_robot.html");
        }

        [Benchmark(Baseline = true)]
        public XDocument RobotHtmlToXmlViaString()
        {
            var parser = new HtmlToXmlParser();
            return parser.ParseViaString(RobotHtml);
        }

        [Benchmark]
        public XDocument RobotHtmlToXmlViaSpan()
        {
            var parser = new HtmlToXmlParser();
            return parser.ParseViaSpan(RobotHtml);
        }

        [Benchmark]
        public XDocument HumanHtmlToXmlViaString()
        {
            var parser = new HtmlToXmlParser();
            return parser.ParseViaString(HumanHtml);
        }

        [Benchmark]
        public XDocument HumanHtmlToXmlViaSpan()
        {
            var parser = new HtmlToXmlParser();
            return parser.ParseViaSpan(HumanHtml);
        }
    }
}
