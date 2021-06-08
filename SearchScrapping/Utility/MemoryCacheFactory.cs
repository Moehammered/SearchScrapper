using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace SearchScraping.Utility
{
    public static class MemoryCacheFactory
    {
        private const int MaxItemLimit = 1000;
        private static IDictionary<Type, IMemoryCache> Caches = new Dictionary<Type, IMemoryCache>();

        public static IMemoryCache FetchCache<T>(int itemLimit = 100)
            => FetchCache<T>(new TimeSpan(0, 1, 0), itemLimit);

        public static IMemoryCache FetchCache<T>(TimeSpan scanFrequency, int itemLimit = 100)
        {
            var type = typeof(T);
            if (Caches.ContainsKey(type))
                return Caches[type];
            else
            {
                var validatedLimit = Math.Max(Math.Min(itemLimit, MaxItemLimit), 1);
                var options = new MemoryCacheOptions()
                {
                    ExpirationScanFrequency = scanFrequency,
                    SizeLimit = validatedLimit
                };
                var cache = new MemoryCache(options);
                Caches.TryAdd(type, cache);
                return cache;
            }
        }

        public static void DisposeAll()
        {
            foreach (var cache in Caches)
                cache.Value.Dispose();
        }
    }
}
