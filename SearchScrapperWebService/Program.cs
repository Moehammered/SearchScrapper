using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SearchScraping.Utility;

namespace SearchScraperWebService
{
    public class Program
    {
        public static void Main(string[] args)
        { 
            CreateHostBuilder(args).Build().Run();
            MemoryCacheFactory.DisposeAll();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureAppConfiguration(options => 
                {
                    options.AddJsonFile("RegexTemplates.json")
                           .AddJsonFile("SearchEngineConfiguration.json")
                           .AddJsonFile("SearchClientHeaders.json");
                });
    }
}
