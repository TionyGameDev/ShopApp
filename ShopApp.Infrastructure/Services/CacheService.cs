using System.Text.Json;
using ShopApp.Application.Interfaces;
using StackExchange.Redis;

namespace ShopApp.Infrastructure.Services;

public class CacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key)
    {
        var result = await _db.StringGetAsync(key);
        if (!result.HasValue)
            return default;
        
        var json = JsonSerializer.Deserialize<T>(result!);
        return json;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key,json,expiry,When.Always);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}