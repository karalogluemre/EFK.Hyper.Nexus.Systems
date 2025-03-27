using Commons.Application.Repositories.Queries;
using Commons.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Commons.Persistence.Repositories.CrudRepositories.Queries
{
    public class ReadRepository<TContext, TEntity>(TContext context) : IReadRepository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
    {
        private readonly TContext context = context;

        public DbSet<TEntity> Table => this.context.Set<TEntity>();

        public IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Table.AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query;
        }
        public IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>> method)
        {
            var query = Table.Where(method);
            return query;
        }
        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> method)
        {
            var query = Table.AsQueryable();
            return await query.FirstOrDefaultAsync(method);
        }
        public async Task<TEntity> GetByIdAsync(string id)
        {
            var entityType = this.context.Model.FindEntityType(typeof(TEntity))?.ClrType;

            if (entityType == null)
                throw new Exception("Entity type could not be found.");

            var guid = Guid.Parse(id);
            var query = Table.AsQueryable();

            return await query.FirstOrDefaultAsync(data =>
                EF.Property<Guid>(data, "Id") == guid
            );
        }

        //public IQueryable<TEntity> GetAllPagination(int? pageNumber, int? pageSize, params Expression<Func<TEntity, object>>[]? includes)
        //{
        //    var query = Table.AsQueryable();
        //    if (pageNumber == 0)
        //    {
        //        pageNumber = 1;
        //    }
        //    if (pageSize == 0)
        //    {
        //        pageSize = 10;
        //    }
        //    // İlişkileri sorguya dahil et
        //    if (includes != null && includes.Any())
        //    {
        //        query = includes.Aggregate(query, (current, include) => current.Include(include));
        //    }

        //    // Varsayılan sıralama ekle
        //    query = query.OrderBy(e => e.Id); // Burada Id'ye göre sıralama yapılır (Primary Key veya uygun bir alan seçin)

        //    // Sayfalama uygula
        //    if (pageNumber.HasValue && pageSize.HasValue)
        //    {
        //        return Pagination.ApplyPagination(query, pageNumber, pageSize);
        //    }

        //    return query;
        //}

    }
}
