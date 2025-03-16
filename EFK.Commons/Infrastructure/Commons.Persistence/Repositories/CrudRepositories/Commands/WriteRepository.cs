using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
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

        public async Task<BaseResponse> AddAsync(TEntity entity)
        {
            try
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

                return new BaseResponse { Succeeded = true, Message = "Kayıt başarıyla eklendi.", Data = entity };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Succeeded = false, Message = $"Kayıt eklenirken hata oluştu: {ex.Message}" };
            }
        }

        public async Task<BaseResponse> AddBulkAsync(ICollection<TEntity> entities)
        {
            try
            {
                this.context.ChangeTracker.AutoDetectChangesEnabled = false;

                // Ana nesneleri ekleyelim
                await this.context.BulkInsertAsync(entities);

                // İç içe geçmiş ilişkili koleksiyonları topla
                var subEntities = new List<TEntity>();

                foreach (var entity in entities)
                {
                    CollectNestedEntities(entity, subEntities);
                }

                // Eğer alt nesneler varsa onları da ekle
                if (subEntities.Any())
                {
                    await this.context.BulkInsertAsync(subEntities);
                }

                this.context.ChangeTracker.AutoDetectChangesEnabled = true;

                // Elasticsearch'e ekleme
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

                return new BaseResponse { Succeeded = true, Message = $"{entities.Count} kayıt başarıyla eklendi.", Data = entities };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Succeeded = false, Message = $"Toplu kayıt eklenirken hata oluştu: {ex.Message}" };
            }
        }
        private void CollectNestedEntities(object entity, List<TEntity> subEntities)
        {
            if (entity == null)
                return;

            var entityType = entity.GetType();
            var properties = entityType.GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(prop.PropertyType.GetGenericTypeDefinition()))
                {
                    var genericType = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                    if (genericType == typeof(TEntity))
                    {
                        var nestedEntities = prop.GetValue(entity) as IEnumerable<TEntity>;
                        if (nestedEntities != null)
                        {
                            foreach (var nestedEntity in nestedEntities)
                            {
                                subEntities.Add(nestedEntity);
                                CollectNestedEntities(nestedEntity, subEntities);
                            }
                        }
                    }
                }
            }
        }

        public async Task<BaseResponse> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await this.Table.FindAsync(id);
                if (entity != null)
                {
                    this.Table.Remove(entity);
                    await this.context.SaveChangesAsync();

                    return new BaseResponse { Succeeded = true, Message = "Kayıt başarıyla silindi." };
                }

                return new BaseResponse { Succeeded = false, Message = "Silinecek kayıt bulunamadı." };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Succeeded = false, Message = $"Kayıt silinirken hata oluştu: {ex.Message}" };
            }
        }

        public async Task<BaseResponse> UpdateAsync(TEntity data)
        {
            try
            {
                this.Table.Update(data);
                await this.context.SaveChangesAsync();
                return new BaseResponse { Succeeded = true, Message = "Kayıt başarıyla güncellendi.", Data = data };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Succeeded = false, Message = $"Kayıt güncellenirken hata oluştu: {ex.Message}" };
            }
        }
    }
}
