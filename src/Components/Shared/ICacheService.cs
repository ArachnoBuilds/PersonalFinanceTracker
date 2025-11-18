namespace Components.Shared;

public interface ICacheService
{
    // TODO check if ValueTask can be used instead
    Task<bool> SetAsync<T>(string key, T value);
    Task<T?> GetAsync<T>(string key);
}
