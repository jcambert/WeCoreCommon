using MediatR;
using WeCoreCommon.Behaviours;

namespace WeCoreCommon.Cache.Behaviours;



public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>, ICacheable
    where TResponse : HandlerResponse
{
    protected readonly ICache Cache;
    protected readonly ILogger<CachingBehaviour<TRequest, TResponse>> Logger;
    public CachingBehaviour(ICache cache, ILogger<CachingBehaviour<TRequest, TResponse>> logger)
    {
        this.Cache = cache;
        this.Logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = request.GetType();
        Logger.LogInformation($"{requestName} is configured for caching." );
        string cacheKey = $"{requestName}-{request.CacheKey}";
        // Check to see if the item is inside the cache
        TResponse response;
        if (Cache.TryGetValue(cacheKey, out response))
        {
            Logger.LogInformation($"Returning cached value for {cacheKey}." );
            return response;
        }

        // Item is not in the cache, execute request and add to cache
        Logger.LogInformation($"{cacheKey} is not inside the cache, executing request." );
        response = await next();
        await Cache.SetRecordAsync(cacheKey, response);
        return response;
    }
}

