namespace WeCoreCommon.Cache;
public sealed class CacheOptions
{
    public const string CacheSectionName = "wecache";
    public bool UseMemoryCache { get; set; }

    public string RedisConnectionString { get; set; }
}

