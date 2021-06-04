using BenchmarkDotNet.Running;

namespace BenchmarkTests
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<QueryParserRegex>();
        }
    }
}
