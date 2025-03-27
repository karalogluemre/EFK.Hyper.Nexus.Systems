using Commons.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Commands
{
    public interface IWriteRepository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
    {
        Task<BaseResponse> AddAsync(TEntity entity);
        Task<BaseResponse> AddBulkAsync(ICollection<TEntity> entities);
        Task<BaseResponse> UpdateBulkAsync(ICollection<TEntity> entities);
        Task<BaseResponse> DeleteAsync(Guid id);
        Task<BaseResponse> RemoveRangeAsync(List<TEntity> entities);

    }
}
