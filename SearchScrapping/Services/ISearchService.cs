using SearchScraping.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SearchScraping.Services
{
    public interface ISearchService
    {
        Task<string> FetchQueryResultsAsync(ISearchEngineConfiguration config, string query, uint count, CancellationToken ct);
    }
}
