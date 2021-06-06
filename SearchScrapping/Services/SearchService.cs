using SearchScraping.Interfaces;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SearchScraping.Services
{
    public class SearchService : ISearchService
    {
        private readonly HttpClient Client;
        public SearchService(HttpClient client)
        {
            Client = client;
        }

        public Task<string> FetchQueryResultsAsync(ISearchEngineConfiguration config, string query, uint count, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Task.FromResult(string.Empty);
            else
            {
                var url = BuildUrl(config, query, count);
                return Client.GetStringAsync(url, ct);
            }
        }

        private string BuildUrl(ISearchEngineConfiguration engine, string query, uint count)
        {
            count = count < engine.MaxCount ? count : engine.MaxCount;

            var countParam = $"{engine.CountParam}={count}";
            var queryParam = $"{engine.QueryParam}={HttpUtility.UrlEncode(query)}";

            return $"{engine.SearchUrl}{queryParam}&{countParam}";
        }
    }
}
