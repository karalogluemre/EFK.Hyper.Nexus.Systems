using Commons.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Commons.Application.Repositories.Queries
{
    public interface IReadRepository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
    {
        IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[]? includes);
        //IQueryable<TEntity> GetAllPagination(int? pageNumber, int? pageSize, params Expression<Func<TEntity, object>>[]? includes);
        IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>> method);
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> method);
        Task<TEntity> GetByIdAsync(string id);
    }
}
