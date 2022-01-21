namespace WeCoreCommon.Cache;


public interface ICache
{
    Task<bool> SetRecordAsync<T>(string key, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpirationTime = null);
    bool TryGetValue<T>(object key, out T value);
    Task<(bool, T)> TryGetValueAsync<T>(object key, CancellationToken token = default);
    void Remove(object key);
}

internal class Cache : ICache, IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private bool disposedValue;

    public Cache(IMemoryCache memoryCache = null, IDistributedCache distributedCache = null)
    {
        this._memoryCache = memoryCache;
        this._distributedCache = distributedCache;
    }

    /// <summary>
    /// Set T Data to cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="absoluteExpireTime"></param>
    /// <param name="slidingExpirationTime"></param>
    /// <returns></returns>
    public async Task<bool> SetRecordAsync<T>(string key, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpirationTime = null)
    {
        if (key == null || (_distributedCache == null && _memoryCache == null))
            return await Task.FromResult<bool>(false);
        if (_memoryCache != null)
        {
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            cacheEntryOptions.SetSlidingExpiration(slidingExpirationTime ?? TimeSpan.FromMinutes(1));
            cacheEntryOptions.AbsoluteExpiration = DateTimeOffset.Now + (absoluteExpireTime ?? TimeSpan.FromMinutes(1));
            _memoryCache.Set(key, data, cacheEntryOptions);
            return await Task.FromResult<bool>(true);
        }
        await _distributedCache.SetRecordAsync(key, data, absoluteExpireTime, slidingExpirationTime);
        return await Task.FromResult<bool>(true);
    }

    //
    // Résumé :
    //     Removes the object associated with the given key.
    //
    // Paramètres :
    //   key:
    //     An object identifying the entry.
    public void Remove(object key)
    {
        _memoryCache?.Remove(key);
        _distributedCache?.Remove(key.ToString());
    }

    //
    // Résumé :
    //     Gets the item associated with this key if present.
    //
    // Paramètres :
    //   key:
    //     An object identifying the requested entry.
    //
    //   value:
    //     The located value or null.
    //
    // Retourne :
    //     True if the key was found.

    public bool TryGetValue<T>(object key, out T value)
    {
        if (key == null || (_distributedCache == null && _memoryCache == null))
        {
            value = default(T);
            return false;
        }
        if (_memoryCache != null)
        {
            try
            {
                return _memoryCache.TryGetValue(key, out value);
            }
            catch (Exception)
            {
                value = default(T);
                return false;
            }

        }
        try
        {
            value = _distributedCache.GetRecord<T>(key.ToString());
            return value != null;

        }
        catch (Exception)
        {

            value = default(T);
            return false;
        }
    }

    //
    // Résumé :
    //     Gets the item associated with this key if present.
    //
    // Paramètres :
    //   key:
    //     An object identifying the requested entry.
    //
    //   value:
    //     The located value or null.
    //
    // Retourne :
    //     True if the key was found.
    public async Task<(bool, T)> TryGetValueAsync<T>(object key, CancellationToken token = default)
    {
        T t_result;
        Func<bool, T, Task<(bool, T)>> found = (bool f, T v) => Task.FromResult<(bool, T)>((f, v));
        Func<Task<(bool, T)>> not_found = () => found(false, default(T));

        if (key == null || (_distributedCache == null && _memoryCache == null))
            return await not_found();
        if (_memoryCache != null)
        {
            var res = _memoryCache.TryGetValue(key, out var value);
            if (res && value is T)
            {
                t_result = (T)value;
                return await found(res, t_result);
            }
            return await not_found();
        }
        try
        {
            t_result = await _distributedCache.GetRecordAsync<T>(key.ToString(), token);
            return await found(t_result != null, t_result);

        }
        catch (Exception)
        {
            return await not_found();

        }
    }

    #region Disposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: supprimer l'état managé (objets managés)
                //DO ?OT DISPOSE MEMORY CACHE
                //this._memoryCache?.Dispose();

            }

            // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
            // TODO: affecter aux grands champs une valeur null
            disposedValue = true;
        }
    }

    // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
    // ~CacheService()
    // {
    //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}

