using Commons.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Commands
{
    public interface IElasticSearchWriteRepository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
    {
        Task<BaseResponse> AddToElasticSearchAsync(TEntity entity);
        Task<BaseResponse> BulkAddToElasticSearchAsync(IEnumerable<TEntity> data);
        Task<BaseResponse> BulkUpdateToElasticSearchAsync(IEnumerable<TEntity> data);
        Task<BaseResponse> BulkDeleteFromElasticSearchAsync(IEnumerable<TEntity> data);
        Task<BaseResponse> SingleUpdateToElasticSearchAsync(TEntity entity);

    }
}
