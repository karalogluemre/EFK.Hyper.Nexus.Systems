namespace Commons.Application.Repositories.Commands
{
    public interface IRedisWriteRepository
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }
}
