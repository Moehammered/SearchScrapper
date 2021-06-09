using SearchScraping.Interfaces;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchScraping.Services
{
    public class DummySearchService : ISearchService
    {
        public DummySearchService(HttpClient client)
        {
        }

        public Task<string> FetchQueryResultsAsync(ISearchEngineConfiguration config, string query, uint count, CancellationToken ct)
        {
            return Task.FromResult("dummy result");
        }
    }
}
