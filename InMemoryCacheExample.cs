// Using MemoryCache to cache frequently accessed data in-memory
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

public class InMemoryCacheExample
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public InMemoryCacheExample()
    {
        // Configure cache expiration and priority
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30)) // Cache expires if not accessed for 30 minutes
            .SetPriority(CacheItemPriority.High);
    }

    // Method to get data from cache or load it if missing
    public List<string> GetFrequentlyAccessedData()
    {
        const string cacheKey = "FrequentlyAccessedData";

        // Try to get data from cache
        if (!_cache.TryGetValue(cacheKey, out List<string> data))
        {
            // Simulate data retrieval, e.g. from database or external service
            data = LoadDataFromDataSource();

            // Store data in cache
            _cache.Set(cacheKey, data, _cacheOptions);
        }

        return data;
    }

    // Simulated method to load data from a data source
    private List<string> LoadDataFromDataSource()
    {
        // In real implementation, this would be a DB call or external API call
        return new List<string> { "Value1", "Value2", "Value3" };
    }
}
