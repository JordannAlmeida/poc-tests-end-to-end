using blood_donate_api.Repository.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace blood_donate_api.Repository
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _cache;

        public CacheRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> TrySetValueAsync<T>(string key, T value, int lifeTimeMinutes, JsonSerializerOptions jsonSerializerOptions)
        {
            DistributedCacheEntryOptions options = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(lifeTimeMinutes)
            };
            try
            {
                string serializedObject = JsonSerializer.Serialize<T>(value);
                await _cache.SetStringAsync(key, serializedObject, options);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<T?> TryGetValueAsync<T>(string key, JsonSerializerOptions jsonSerializerOptions)
        {
            try
            {
                string? serializedObject = await _cache.GetStringAsync(key);
                if (serializedObject != null)
                {
                    return JsonSerializer.Deserialize<T>(serializedObject, jsonSerializerOptions);
                }
                return default;
            }
            catch (Exception ex)
            {
                return default;
            }
        }
    }
}
