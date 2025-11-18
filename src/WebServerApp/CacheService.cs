using Components.Shared;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WebServerApp;

public class CacheService(ProtectedLocalStorage cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var result = await cache.GetAsync<T>(key);
        return result.Success ? result.Value : default;
    }

    public async Task<bool> SetAsync<T>(string key, T value)
    {
        if (value == null)
            return false;
        try
        {
            await cache.SetAsync(key, value);
        }
        catch
        {
            return false;
        }
        return true;
    }
}
