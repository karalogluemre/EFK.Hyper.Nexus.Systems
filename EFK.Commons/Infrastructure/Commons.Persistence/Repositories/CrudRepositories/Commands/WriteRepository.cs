using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Nest;
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

        public async Task<BaseResponse> AddOrUpdateBulkAsync(ICollection<TEntity> entities)
        {
            try
            {
                var toInsert = new List<TEntity>();
                var toUpdate = new List<TEntity>();

                foreach (var entity in entities)
                {
                    var propertyInfo = entity.GetType().GetProperty("Id");

                    if (propertyInfo == null) continue;

                    var idValue = (Guid)propertyInfo.GetValue(entity);

                    if (idValue == Guid.Empty)
                    {
                        var newId = Guid.NewGuid();
                        propertyInfo.SetValue(entity, newId);
                        toInsert.Add(entity);
                    }
                    else
                    {
                        // Varlığını kontrol et
                        var existing = await this.context.Set<TEntity>().FindAsync(idValue);
                        if (existing != null)
                        {
                            this.context.Entry(existing).CurrentValues.SetValues(entity);
                            toUpdate.Add(entity);
                        }
                        else
                        {
                            toInsert.Add(entity); // Kayıt yoksa eklemeye al
                        }
                    }
                }

                this.context.ChangeTracker.AutoDetectChangesEnabled = false;

                if (toInsert.Any())
                    await this.context.BulkInsertAsync(toInsert);

                if (toUpdate.Any())
                    await this.context.BulkUpdateAsync(toUpdate);

                await this.context.SaveChangesAsync();

                this.context.ChangeTracker.AutoDetectChangesEnabled = true;

                // Elasticsearch senkronizasyonu
                foreach (var entity in entities)
                {
                    try
                    {
                        await this.elasticSearchRepository.AddToElasticSearchAsync(entity);
                    }
                    catch (Exception)
                    {
                        var json = JsonConvert.SerializeObject(entity);
                        var entityName = typeof(TEntity).Name;
                        rabbitMQProducer.Publish(json, entityName);
                    }
                }

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = $"{entities.Count} kayıt başarıyla işlendi.",
                    Data = entities
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Hata: {ex.Message}"
                };
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
                if (entity == null)
                {
                    return new BaseResponse
                    {
                        Succeeded = false,
                        Message = "Silinecek kayıt bulunamadı."
                    };
                }

                // Soft delete işlemi
                var isDeletedProp = entity.GetType().GetProperty("IsDeleted");
                if (isDeletedProp != null)
                {
                    isDeletedProp.SetValue(entity, true);
                }

                // UpdatedDate ayarı
                var updatedDateProp = entity.GetType().GetProperty("UpdatedDate");
                if (updatedDateProp != null)
                {
                    updatedDateProp.SetValue(entity, DateTime.UtcNow);
                }

                this.context.Update(entity);
                await this.context.SaveChangesAsync();

                try
                {
                    // Elasticsearch üzerinde de güncelleme yap (soft delete yansıtılması için)
                    await this.elasticSearchRepository.BulkUpdateToElasticSearchAsync(new List<TEntity> { entity });
                }
                catch (Exception)
                {
                    var jsonData = JsonConvert.SerializeObject(entity);
                    var entityName = typeof(TEntity).Name;
                    this.rabbitMQProducer.Publish(jsonData, entityName);
                }

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Kayıt başarıyla silindi.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Kayıt silinirken hata oluştu: {ex.Message}"
                };
            }
        }


        public async Task<BaseResponse> UpdateBulkAsync(ICollection<TEntity> entities)
        {
            try
            {
                this.context.ChangeTracker.AutoDetectChangesEnabled = false;
                await this.context.BulkUpdateAsync(entities);
                this.context.ChangeTracker.AutoDetectChangesEnabled = true;

                try
                {
                    // Elasticsearch'te de güncelleme yap
                    await this.elasticSearchRepository.BulkUpdateToElasticSearchAsync(entities);
                }
                catch (Exception)
                {
                    // Elasticsearch kapalıysa RabbitMQ'ya mesaj gönder
                    foreach (var entity in entities)
                    {
                        var jsonData = JsonConvert.SerializeObject(entity);
                        var entityName = typeof(TEntity).Name;
                        this.rabbitMQProducer.Publish(jsonData, entityName);
                    }
                }

                return new BaseResponse { Succeeded = true, Message = $"{entities.Count} kayıt başarıyla güncellendi.", Data = entities };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Succeeded = false, Message = $"Toplu kayıt güncellenirken hata oluştu: {ex.Message}" };
            }
        }
        public async Task<BaseResponse> RemoveAsync(List<TEntity> entities)
        {
            try
            {
                this.context.Set<TEntity>().RemoveRange(entities);
                await this.context.SaveChangesAsync();

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Kayıtlar başarıyla silindi"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Silme sırasında hata: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse> RemoveRangeAsync(List<TEntity> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    var isDeletedProp = entity.GetType().GetProperty("IsDeleted");
                    if (isDeletedProp != null)
                    {
                        isDeletedProp.SetValue(entity, true);
                    }

                    var updatedDateProp = entity.GetType().GetProperty("UpdatedDate");
                    if (updatedDateProp != null)
                    {
                        updatedDateProp.SetValue(entity, DateTime.UtcNow);
                    }
                }

                this.context.ChangeTracker.AutoDetectChangesEnabled = false;
                await this.context.BulkUpdateAsync(entities);
                this.context.ChangeTracker.AutoDetectChangesEnabled = true;

                try
                {
                    await this.elasticSearchRepository.BulkUpdateToElasticSearchAsync(entities);
                }
                catch (Exception)
                {
                    foreach (var entity in entities)
                    {
                        var jsonData = JsonConvert.SerializeObject(entity);
                        var entityName = typeof(TEntity).Name;
                        this.rabbitMQProducer.Publish(jsonData, entityName);
                    }
                }

                return new BaseResponse { Succeeded = true, Message = "Toplu silme işlemi başarıyla gerçekleştirildi.", Data = entities };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Succeeded = false, Message = $"Toplu silme işlemi sırasında hata oluştu: {ex.Message}" };
            }
        }
    }
}
