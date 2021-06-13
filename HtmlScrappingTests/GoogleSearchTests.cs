using HtmlScrappingTests.DemoModels;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace HtmlScrapingTests
{
    [TestClass]
    public class GoogleSearchTests
    {
        private readonly SearchRequest ConveyancingSearch = new SearchRequest()
        {
            SearchTerm = "conveyancing software"
        };

        [TestMethod]
        public async Task GoogleSinglePageRequest()
        {
            var googleEngine = LoadGoogleData();
            var results = await PerformSearchAsync(googleEngine, ConveyancingSearch);

            if (!System.IO.Directory.Exists("google"))
                System.IO.Directory.CreateDirectory("google");
            await System.IO.File.WriteAllTextAsync("google/google-au_allLinks.html", results);

            //var matches = System.Text.RegularExpressions.Regex.Matches(response, QueryResultPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //var successfulMatches = matches.Cast<System.Text.RegularExpressions.Match>().Where(x => x.Success);
            //var joinedLinks = string.Join("\n", successfulMatches.Select(x => x.Value));
            //await System.IO.File.WriteAllTextAsync("google/google-au_allLinks.html_list.txt", joinedLinks);
        }

        [TestMethod]
        public async Task AsyncGooglePageRequestsTest()
        {
            var itemCount = 100;
            var itemsPerPage = 10;

            var baseUrl = @"https://www.google.com.au/search?q=conveyancing+software&";
            var queries = Enumerable.Range(0, itemCount / itemsPerPage)
                .Select(index => $@"{baseUrl}num={itemsPerPage}&start={index * itemsPerPage}");

            using (var client = new HttpClient())
            {
                var jobs = queries.Select(x => client.GetStringAsync(x));

                var pages = await Task.WhenAll(jobs);
                if (!System.IO.Directory.Exists("google"))
                    System.IO.Directory.CreateDirectory("google");

                var fileTasks = pages.Select((x, i) => System.IO.File.WriteAllTextAsync($"google/google-au_{i}.html", x));
                await Task.WhenAll(fileTasks);
            }
        }

        private async Task<string> PerformSearchAsync(SearchEngineConfiguration engine, SearchRequest request)
        {
            var urlQuery = BuildSearchRequest(engine, request);
            using (var client = new HttpClient())
                return await client.GetStringAsync(urlQuery);
        }

        private string BuildSearchRequest(SearchEngineConfiguration engine, SearchRequest request)
        {
            var count = engine.MaxCount < request.Count ? engine.MaxCount : request.Count;
            var countParam = $"{engine.CountParam}={count}";
            var queryParam = $"{engine.QueryParam}={HttpUtility.UrlEncode(request.SearchTerm)}";

            return $"{engine.SearchUrl}{queryParam}&{countParam}";
        }

        private SearchEngineConfiguration LoadGoogleData()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("SearchEngineConfiguration.json")
                .Build();

            var searchEngines = config.GetSection("searchEngines")
                .Get<IEnumerable<SearchEngineConfiguration>>();

            return searchEngines.First(x => x.Name.Equals("google-au", System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
