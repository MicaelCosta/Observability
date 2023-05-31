namespace Core.Caching
{
    public interface ICachingService
    {
        Task SetAsync<T>(string key, T value);

        Task<T> GetAsync<T>(string key);

        Task RemoveAsync(string key);
    }
}
