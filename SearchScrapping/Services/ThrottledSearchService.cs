using SearchScraping.Interfaces;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SearchScraping.Services
{
    public class ThrottledSearchService : ISearchService
    {
        private readonly HttpClient Client;
        private readonly Random Randomiser;
        private const int MinMilli = 800;
        private const int MaxMilli = 2000;

        public ThrottledSearchService(HttpClient client)
        {
            Client = client;
            Randomiser = new Random();
        }

        public async Task<string> FetchQueryResultsAsync(ISearchEngineConfiguration config, string query, uint count, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
                return string.Empty;
            else
            {
                var url = BuildUrl(config, query, count);
                await Task.Delay(Randomiser.Next(MinMilli, MaxMilli));
                return await Client.GetStringAsync(url, ct);
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
