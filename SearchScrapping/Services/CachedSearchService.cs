using Microsoft.Extensions.Caching.Memory;
using SearchScraping.Interfaces;
using SearchScraping.Utility;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchScraping.Services
{
    public class CachedSearchService : ISearchService
    {
        private readonly ThrottledSearchService Searcher;
        private readonly IMemoryCache Cache;

        public CachedSearchService(HttpClient client)
        {
            Searcher = new ThrottledSearchService(client);
            Cache = MemoryCacheFactory.FetchCache<string>();
        }

        public async Task<string> FetchQueryResultsAsync(ISearchEngineConfiguration config, string query, uint count, CancellationToken ct)
        {
            var accessKey = (config, query, count);
            if (Cache.TryGetValue(accessKey, out string item))
                return item;
            else
            {
                var result = await Searcher.FetchQueryResultsAsync(config, query, count, ct);
                Cache.Set(accessKey, result, CreateCacheItemOptions());
                return result;
            }
        }

        private MemoryCacheEntryOptions CreateCacheItemOptions()
        {
            return new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now + new TimeSpan(0, 60, 0),
                SlidingExpiration = new TimeSpan(0, 10, 0),
                Size = 1
            };
        }
    }
}
