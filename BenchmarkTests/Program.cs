using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BenchmarkTests
{
    class Program
    {
        private static Dictionary<string, Type> Benchmarks = new Dictionary<string, Type>()
        {
            { "1", typeof(LinkRegexParser) },
            { "2", typeof(LinkHeadingRegexParser) },
            { "3", typeof(QueryParserRegex) },
            { "4", typeof(RegexComparisonBenchmark) },
            { "5", typeof(FallbackParserBenchmark) },
            { "6", typeof(HtmlStrippingBenchmark) },
            { "7", typeof(HtmlToXmlBenchmark) }
        };

        private static string BuildOptions(IDictionary<string, Type> options)
        {
            var items = Benchmarks.Select(x => $"{x.Key} - {x.Value.Name}");
            return string.Join("\n", items);
        }

        private static string RequestChoice()
        {
            var options = BuildOptions(Benchmarks);
            Console.WriteLine(options);
            Console.WriteLine();
            Console.WriteLine("Choose which benchmark you'd like run(or q to quit):");
            return Console.ReadLine()?.ToLower() ?? "";
        }

        static void Main(string[] args)
        {
            var choice = "";
            while (choice != "q")
            {
                Console.Clear();
                choice = RequestChoice();
                if (Benchmarks.ContainsKey(choice))
                {
                    BenchmarkRunner.Run(Benchmarks[choice]);
                    break;
                }
            }
#if DEBUG
            var runner = new LinkHeadingRegexParser();
            runner.Setup();
            runner.TargetUrl = "www.smokeball.com.au";
            runner.FindDomainPosition();
#else
            //BenchmarkRunner.Run<RegexComparisonBenchmark>();
            //BenchmarkRunner.Run<FallbackParserBenchmark>();
#endif
        }
    }
}
