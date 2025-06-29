using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Common;
using Commons.Application.Repositories.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Commons.Persistence.Repositories.Common
{
    public class UnitOfWork<TContext>(TContext context, IServiceProvider serviceProvider) : IUnitOfWork<TContext>
 where TContext : DbContext
    {
        public TContext DbContext => context;
        readonly public IServiceProvider serviceProvider = serviceProvider;

        public IWriteRepository<TContext, TEntity> GetWriteRepository<TEntity>() where TEntity : class =>
             this.serviceProvider.GetRequiredService<IWriteRepository<TContext, TEntity>>();

        public IReadRepository<TContext, TEntity> GetReadRepository<TEntity>() where TEntity : class =>
             this.serviceProvider.GetRequiredService<IReadRepository<TContext, TEntity>>();

        public IElasticSearchReadRepository<TContext, TEntity> GetElasticReadRepository<TEntity>() where TEntity : class =>
             serviceProvider.GetRequiredService<IElasticSearchReadRepository<TContext, TEntity>>();

        public IElasticSearchWriteRepository<TContext, TEntity> GetElasticWriteRepository<TEntity>() where TEntity : class =>
            serviceProvider.GetRequiredService<IElasticSearchWriteRepository<TContext, TEntity>>();


        public IRedisReadRepository GetRedisReadRepository() =>
            serviceProvider.GetRequiredService<IRedisReadRepository>();

        public IRedisWriteRepository GetRedisWriteRepository() =>
            serviceProvider.GetRequiredService<IRedisWriteRepository>();

        public async Task<int> CommitAsync() => await context.SaveChangesAsync();

        public async ValueTask DisposeAsync() => await context.DisposeAsync();

        public IMongoReadRepository<TContext> GetMongoReadRepository() =>
            serviceProvider.GetRequiredService<IMongoReadRepository<TContext>>();
        public IMongoWriteRepository GetMongoWriteRepository() =>
            serviceProvider.GetRequiredService<IMongoWriteRepository>();
    }
}