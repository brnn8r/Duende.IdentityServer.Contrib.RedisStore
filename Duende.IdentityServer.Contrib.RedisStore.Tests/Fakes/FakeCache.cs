using Duende.IdentityServer.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Duende.IdentityServer.Contrib.RedisStore.Tests.Cache
{
    public class FakeCache<T> : ICache<T> where T : class
    {
        private readonly IMemoryCache cache;

        private readonly ILogger<FakeCache<T>> logger;

        public FakeCache(IMemoryCache memoryCache, FakeLogger<FakeCache<T>> logger)
        {
            cache = memoryCache;
            this.logger = logger;
        }

        public Task<T> GetAsync(string key)
        {
            var result = cache.Get(key);

            if (result == null)
                logger.LogDebug($"Cache miss for {key}");
            else
                logger.LogDebug($"Cache hit for {key}");

            return Task.FromResult((T)result);
        }

        public async Task<T> GetOrAddAsync(string key, TimeSpan expiration, Func<Task<T>> get)
        {
            var result = await GetAsync(key);

            if(result != default)
            {
                return result;
            }

            if(get == null || (result = await get()) == default)
            {
                return default;
            }

            await SetAsync(key, result, expiration);

            return result;

        }

        public Task RemoveAsync(string key)
        {
            cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task SetAsync(string key, T item, TimeSpan expiration)
        {
            cache.Set(key, item, expiration);
            return Task.CompletedTask;
        }

    }
}
