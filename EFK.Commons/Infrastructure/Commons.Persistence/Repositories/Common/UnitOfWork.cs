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
        readonly private TContext context = context;
        readonly private IServiceProvider serviceProvider = serviceProvider; 

        public IWriteRepository<TContext, TEntity> GetWriteRepository<TEntity>() where TEntity : class
        {
            return this.serviceProvider.GetRequiredService<IWriteRepository<TContext, TEntity>>();
        }

        public IReadRepository<TContext, TEntity> GetReadRepository<TEntity>() where TEntity : class
        {
            return this.serviceProvider.GetRequiredService<IReadRepository<TContext, TEntity>>();
        }

        public async Task<int> CommitAsync()
        {
            return await this.context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await this.context.DisposeAsync();
        }
    }
}
