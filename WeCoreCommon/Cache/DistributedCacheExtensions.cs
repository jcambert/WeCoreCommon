

using System.Text.Json;

namespace WeCoreCommon.Cache;

public static class DistributedCacheExtension
{
    public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recodeId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpirationTime = null)
    {
        var options = new DistributedCacheEntryOptions();
        options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
        options.SlidingExpiration = slidingExpirationTime;
        var jsonData = JsonSerializer.Serialize(data);
        await cache.SetStringAsync(recodeId, jsonData, options);
    }

    public static T? GetRecord<T>(this IDistributedCache cache, string recordId)
    {
        var jsonData = cache.GetString(recordId);
        if (jsonData is null)
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId, CancellationToken token = default)
    {
        var jsonData = await cache.GetStringAsync(recordId, token);
        if (jsonData is null)
        {
            return default(T);
        }
        return JsonSerializer.Deserialize<T>(jsonData);
    }
}
