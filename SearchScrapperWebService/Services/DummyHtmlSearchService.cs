using SearchScraping.Interfaces;
using SearchScraping.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchScraperWebService.Services
{
    public class DummyHtmlSearchService : ISearchService
    {
        private const string HtmlFile = @"dummy-data/google-au_allLinks_robot.html";
        private readonly string Html;
        public DummyHtmlSearchService(HttpClient client)
            => Html = System.IO.File.ReadAllText(HtmlFile);

        public Task<string> FetchQueryResultsAsync(ISearchEngineConfiguration config, string query, uint count, CancellationToken ct)
            => Task.FromResult(Html);
    }
}
