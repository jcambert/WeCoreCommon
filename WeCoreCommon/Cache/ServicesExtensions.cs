using MediatR;
using System.Reflection;
using WeCoreCommon.Cache.Behaviours;

namespace WeCoreCommon.Cache;


public static class ServicesExtensions
{

    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration config, Action<CacheOptions> configure = null,params Assembly[] handlderAssemblies)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        services.Configure<CacheOptions>(config.GetSection(CacheOptions.CacheSectionName));

       
        CacheOptions opt = new CacheOptions();
        configure?.Invoke(opt);
        config.Bind(CacheOptions.CacheSectionName, opt);

        var hasInMemoryCache = opt.UseMemoryCache;
        var hasDistributedCache = !hasInMemoryCache && (!string.IsNullOrEmpty(opt.RedisConnectionString) || !string.IsNullOrEmpty(config.GetConnectionString("Redis")));

        if (hasInMemoryCache)
        {
            services.AddMemoryCache();
        }
        if (hasDistributedCache)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = opt.RedisConnectionString ?? config.GetConnectionString("Redis");
                options.InstanceName = "localRedis_";
            });
        }

        if (hasInMemoryCache || hasDistributedCache)
        {
            /*var z = new Assembly[handlderAssemblies.Length + 1];
            handlderAssemblies.CopyTo(z, 1);
            z[0] = typeof(ServicesExtensions).Assembly;*/

            services.AddScoped<ICache, Cache>();
            //services.AddMediatR(z);
            services.AddMediator(handlderAssemblies);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehaviour<,>));
        }
        return services;
    }
}

