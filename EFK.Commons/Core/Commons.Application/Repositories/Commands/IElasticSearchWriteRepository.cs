using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Commands
{
    public interface IElasticSearchWriteRepository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
    {
        Task AddToElasticSearchAsync(TEntity data);
        Task BulkAddToElasticSearchAsync(IEnumerable<TEntity> data);
        Task BulkDeleteFromElasticSearchAsync(IEnumerable<TEntity> data); 

    }
}
