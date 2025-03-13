using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Commands
{
    public interface IWriteRepository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
    {
        Task AddAsync(TEntity data);
        Task AddBulkAsync(ICollection<TEntity> datas);
        Task UpdateAsync(TEntity data);
        Task DeleteAsync(Guid id);
    }
}
