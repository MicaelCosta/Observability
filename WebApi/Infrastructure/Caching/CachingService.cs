using Core.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Caching
{
    public class CachingService : ICachingService
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _options;

        public CachingService(IDistributedCache cache)
        {
            _cache = cache;
            _options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600), //uma hora, tempo absoluto para expirar
                SlidingExpiration = TimeSpan.FromSeconds(1200),//20 minutos sem ser acessado, expira
            };
        }

        public async Task<T> GetAsync<T>(string key)
        {
            string objectCache = await _cache.GetStringAsync(key);
            if (string.IsNullOrWhiteSpace(objectCache))
                return default(T);

            return JsonSerializer.Deserialize<T>(objectCache);
        }

        public async Task SetAsync<T>(string key, T value)
        {
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), _options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
