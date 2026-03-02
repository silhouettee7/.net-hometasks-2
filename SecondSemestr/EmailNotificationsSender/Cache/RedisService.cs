using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;

namespace EmailNotificationsSender.Cache;

public class RedisService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly string _instancePrefix;
    
    public RedisService(string connectionString, string instancePrefix = "email_service")
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
        _instancePrefix = instancePrefix;
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(GetKey(key));
        return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(GetKey(key), json, expiration);
    }
    
    public string GenerateCacheKey(
        string prefix,
        long cacheVersion,
        object parameters)
    {
        var json = JsonSerializer.Serialize(parameters);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        var data = Convert.ToHexString(hash).Substring(0,12);
        return $"{prefix}:v{cacheVersion}:{data}";
    }
    
    public async Task InvalidateCache(string cacheVersionKey)
    {
        var currentVersion = await GetCurrentCacheVersion(cacheVersionKey);
        var newVersion = currentVersion + 1;
        await SetAsync(cacheVersionKey, newVersion, TimeSpan.MaxValue);
    }
    
    public async Task<long> GetCurrentCacheVersion(string cacheVersionKey)
    {
        var version = await GetAsync<long>(cacheVersionKey);
        return version == 0 ? 1 : version;
    }
    
    private string GetKey(string key) => $"{_instancePrefix}:{key}";
}