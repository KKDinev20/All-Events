using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AllEvents.TicketManagement.Application.Extensions
{
    public static class DistributedCacheMethodsExtensions
    {
        private static readonly string KeyTrackingListPrefix = "KeyTrackingList:";

        public static async Task SetCacheAsync(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options, string prefix)
        {
            var cacheKey = $"{prefix}:{key}";
            var jsonData = JsonConvert.SerializeObject(value);
            await cache.SetStringAsync(cacheKey, jsonData, options);

            await AddKeyToTrackingListAsync(cache, prefix, cacheKey);
        }

        public static async Task<T?> GetCacheAsync<T>(this IDistributedCache cache, string key, string prefix)
        {
            var cacheKey = $"{prefix}:{key}";
            var jsonData = await cache.GetStringAsync(cacheKey);
            return jsonData == null ? default : JsonConvert.DeserializeObject<T>(jsonData);
        }

        public static async Task RemoveCacheAsync(this IDistributedCache cache, string key, string prefix)
        {
            var cacheKey = $"{prefix}:{key}";
            await cache.RemoveAsync(cacheKey);
            await RemoveKeyFromTrackingListAsync(cache, prefix, cacheKey);
        }

        public static async Task<IEnumerable<string>> GetCacheKeysStartingWithAsync(this IDistributedCache cache, string prefix)
        {
            var listKey = $"{KeyTrackingListPrefix}{prefix}";
            var keysString = await cache.GetStringAsync(listKey);
            if (string.IsNullOrEmpty(keysString))
            {
                return Enumerable.Empty<string>();
            }

            return keysString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        private static async Task AddKeyToTrackingListAsync(IDistributedCache cache, string prefix, string cacheKey)
        {
            var listKey = $"{KeyTrackingListPrefix}{prefix}";
            var existingKeys = await cache.GetStringAsync(listKey) ?? string.Empty;
            var keys = existingKeys.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            if (!keys.Contains(cacheKey))
            {
                keys.Add(cacheKey);
                await cache.SetStringAsync(listKey, string.Join(',', keys));
            }
        }

        private static async Task RemoveKeyFromTrackingListAsync(IDistributedCache cache, string prefix, string cacheKey)
        {
            var listKey = $"{KeyTrackingListPrefix}{prefix}";
            var existingKeys = await cache.GetStringAsync(listKey) ?? string.Empty;
            var keys = existingKeys.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            if (keys.Contains(cacheKey))
            {
                keys.Remove(cacheKey);
                await cache.SetStringAsync(listKey, string.Join(',', keys));
            }
        }

        public static async Task InvalidateCacheAsync(this IDistributedCache cache, string prefix)
        {
            var keys = await cache.GetCacheKeysStartingWithAsync(prefix);
            foreach (var key in keys)
            {
                await cache.RemoveAsync(key);
            }
        }
    }
}
