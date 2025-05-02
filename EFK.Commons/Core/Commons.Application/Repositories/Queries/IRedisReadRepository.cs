namespace Commons.Application.Repositories.Queries
{
    public interface IRedisReadRepository
    {
        Task<T?> GetAsync<T>(string key);
    }
}
