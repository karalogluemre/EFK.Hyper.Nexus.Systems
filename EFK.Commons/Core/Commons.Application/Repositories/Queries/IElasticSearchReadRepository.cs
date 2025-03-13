using Commons.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Commons.Application.Repositories.Queries
{
    public interface IElasticSearchReadRepository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
    {
        Task<List<TEntity>> GetAllFromElasticSearchAsync();

    }
}
