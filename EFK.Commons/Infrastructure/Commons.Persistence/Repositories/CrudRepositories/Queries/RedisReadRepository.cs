using Commons.Application.Repositories.Queries;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Commons.Persistence.Repositories.CrudRepositories.Queries
{
    public class RedisReadRepository(IDistributedCache cache) : IRedisReadRepository
    {
        readonly IDistributedCache cache = cache;
        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await this.cache.GetStringAsync(key);
            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        }
    }
}
