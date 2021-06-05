using BenchmarkDotNet.Running;

namespace BenchmarkTests
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var runner = new LinkHeadingRegexParser();
            runner.Setup();
            runner.TargetUrl = "www.smokeball.com.au";
            runner.FindDomainPosition();
#else
            BenchmarkRunner.Run<RegexComparisonBenchmark>();
#endif
        }
    }
}
