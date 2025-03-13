using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class RabbitMQConsumerService<TDbContext, TEntity> : RabbitMQConsumer<TDbContext, TEntity>
    where TDbContext : DbContext
    where TEntity : class
    {
        public RabbitMQConsumerService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
            : base(serviceScopeFactory, configuration)
        {
        }
    }
}
