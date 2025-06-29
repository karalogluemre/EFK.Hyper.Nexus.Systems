using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Common
{
    public interface IUnitOfWork<TContext> : IAsyncDisposable
      where TContext : DbContext
    {
        TContext DbContext { get; }
        IWriteRepository<TContext, TEntity> GetWriteRepository<TEntity>() where TEntity : class;
        IReadRepository<TContext, TEntity> GetReadRepository<TEntity>() where TEntity : class;
        IElasticSearchReadRepository<TContext, TEntity> GetElasticReadRepository<TEntity>() where TEntity : class;
        IElasticSearchWriteRepository<TContext, TEntity> GetElasticWriteRepository<TEntity>() where TEntity : class;
        IRedisReadRepository GetRedisReadRepository();
        IRedisWriteRepository GetRedisWriteRepository();
        IMongoReadRepository<TContext> GetMongoReadRepository();
        IMongoWriteRepository GetMongoWriteRepository();

        Task<int> CommitAsync();
    }
}
