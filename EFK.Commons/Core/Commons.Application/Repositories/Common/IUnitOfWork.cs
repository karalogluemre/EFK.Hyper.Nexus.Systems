using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Common
{
    public interface IUnitOfWork<TContext> : IAsyncDisposable
    where TContext : DbContext
    {
        IWriteRepository<TContext, TEntity> GetWriteRepository<TEntity>() where TEntity : class;
        IReadRepository<TContext, TEntity> GetReadRepository<TEntity>() where TEntity : class;
        Task<int> CommitAsync();
    }
}
