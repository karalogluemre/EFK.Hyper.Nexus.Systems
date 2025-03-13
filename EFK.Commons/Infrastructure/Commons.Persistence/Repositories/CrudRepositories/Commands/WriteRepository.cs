using Commons.Application.Repositories.Commands;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class WriteRepository<TContext, TEntity>(TContext context, IElasticSearchWriteRepository<TContext, TEntity> elasticSearchRepository, IRabbitMQProducer rabbitMQProducer) : IWriteRepository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
    {

        readonly private TContext context = context;
        public DbSet<TEntity> Table => this.context.Set<TEntity>();
        readonly private IElasticSearchWriteRepository<TContext, TEntity> elasticSearchRepository = elasticSearchRepository;
        private readonly IRabbitMQProducer rabbitMQProducer = rabbitMQProducer;

        public async Task AddAsync(TEntity entity)
        {
            await Table.AddAsync(entity);
            await this.context.SaveChangesAsync();

            try
            {
                await this.elasticSearchRepository.AddToElasticSearchAsync(entity);
            }
            catch (Exception)
            {
                var jsonData = JsonConvert.SerializeObject(entity);
                var entityName = typeof(TEntity).Name;
                this.rabbitMQProducer.Publish(jsonData, entityName);
            }
        }

        public async Task AddBulkAsync(ICollection<TEntity> entities)
        {
            this.context.ChangeTracker.AutoDetectChangesEnabled = false;
            await this.context.BulkInsertAsync(entities);
            this.context.ChangeTracker.AutoDetectChangesEnabled = true;

            foreach (var entity in entities)
            {
                try
                {
                    await this.elasticSearchRepository.AddToElasticSearchAsync(entity);
                }
                catch (Exception)
                {
                    var jsonData = JsonConvert.SerializeObject(entity);
                    var entityName = typeof(TEntity).Name;
                    this.rabbitMQProducer.Publish(jsonData, entityName);
                }
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await this.Table.FindAsync(id);
            if (entity != null)
            {
                this.Table.Remove(entity);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(TEntity data)
        {
            this.Table.Update(data);
            await this.context.SaveChangesAsync();
        }
    }
}
