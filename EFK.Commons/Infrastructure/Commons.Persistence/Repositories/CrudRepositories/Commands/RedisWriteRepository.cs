using Commons.Application.Repositories.Commands;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class RedisWriteRepository(IDistributedCache cache) : IRedisWriteRepository
    {
        readonly IDistributedCache cache = cache;

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };

            var json = JsonSerializer.Serialize(value);
            await this.cache.SetStringAsync(key, json, options);
        }
        public async Task RemoveAsync(string key)
        {
            await this.cache.RemoveAsync(key);
        }
    }
}
