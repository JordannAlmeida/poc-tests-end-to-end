using System.Text.Json;

namespace blood_donate_api.Repository.Interfaces
{
    public interface ICacheRepository
    {
        Task<T?> TryGetValueAsync<T>(string key, JsonSerializerOptions jsonSerializerOptions);
        Task<bool> TrySetValueAsync<T>(string key, T value, int lifeTimeMinutes, JsonSerializerOptions jsonSerializerOptions);
    }
}
