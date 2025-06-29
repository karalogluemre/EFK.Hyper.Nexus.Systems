using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;
using WatchDog;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class WriteRepository<TContext, TEntity>(
     TContext context,
     IElasticSearchWriteRepository<TContext, TEntity> elasticSearchRepository,
     IRabbitMQProducer rabbitMQProducer,
     IServiceProvider serviceProvider
     ) : IWriteRepository<TContext, TEntity>
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
                WatchLogger.Log($" {typeof(TEntity).Name} verisi ekleniyor: {JsonConvert.SerializeObject(entity)}");

                await Table.AddAsync(entity);
                await this.context.SaveChangesAsync();

                WatchLogger.Log($" {typeof(TEntity).Name} verisi veritabanına eklendi.");

                try
                {
                    await this.elasticSearchRepository.AddToElasticSearchAsync(entity);
                    WatchLogger.Log($" {typeof(TEntity).Name} verisi Elasticsearch'e eklendi.");
                }
                catch (Exception esEx)
                {
                    var jsonData = JsonConvert.SerializeObject(entity);
                    var entityName = typeof(TEntity).Name;
                    this.rabbitMQProducer.Publish(jsonData, entityName);
                    WatchLogger.Log($" Elasticsearch hatası: {esEx.Message} - Veri RabbitMQ kuyruğuna gönderildi. Entity: {entityName}");
                }

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Kayıt başarıyla eklendi.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                WatchLogger.Log($" Hata oluştu: {ex.Message} - StackTrace: {ex.StackTrace}");
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Kayıt eklenirken hata oluştu: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse> AddOrUpdateBulkAsync(ICollection<TEntity> entities)
        {
            WatchLogger.Log($"Toplam {entities.Count} adet {typeof(TEntity).Name} verisi işlenmeye başlanıyor.");

            try
            {
                var toInsert = new List<TEntity>();
                var toUpdate = new List<TEntity>();

                foreach (var entity in entities)
                {
                    var propertyInfo = entity.GetType().GetProperty("Id");
                    if (propertyInfo == null || propertyInfo.PropertyType != typeof(int))
                    {
                        WatchLogger.LogError("Geçerli 'Id' özelliği bulunamadı, veri atlandı.");
                        continue;
                    }

                    var idValue = (int)propertyInfo.GetValue(entity);

                    var isDeletedProp = entity.GetType().GetProperty("isdeleted", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (isDeletedProp != null &&
                        (isDeletedProp.PropertyType == typeof(bool) || isDeletedProp.PropertyType == typeof(bool?)))
                    {
                        isDeletedProp.SetValue(entity, false);
                    }

                    if (idValue == 0)
                    {
                        var createdAtProp = entity.GetType().GetProperty("CreatedAt");
                        if (createdAtProp != null && createdAtProp.PropertyType == typeof(DateTime))
                        {
                            createdAtProp.SetValue(entity, DateTime.Now);
                        }

                        toInsert.Add(entity);
                    }
                    else
                    {
                        var existing = await this.context.Set<TEntity>().FindAsync(idValue);
                        if (existing != null)
                        {
                            var updatedAtProp = entity.GetType().GetProperty("updatedAt");
                            if (updatedAtProp != null && updatedAtProp.PropertyType == typeof(DateTime))
                            {
                                updatedAtProp.SetValue(entity, DateTime.Now);
                            }

                            this.context.Entry(existing).CurrentValues.SetValues(entity);
                            toUpdate.Add(entity);
                        }
                        else
                        {
                            var createdAtProp = entity.GetType().GetProperty("createdAt");
                            if (createdAtProp != null && createdAtProp.PropertyType == typeof(DateTime))
                            {
                                createdAtProp.SetValue(entity, DateTime.Now);
                            }

                            toInsert.Add(entity);
                        }
                    }
                }

                this.context.ChangeTracker.AutoDetectChangesEnabled = false;

                if (toInsert.Any())
                {
                    await context.BulkInsertAsync(toInsert, new BulkConfig { SetOutputIdentity = true });
                    WatchLogger.Log($"{toInsert.Count} adet yeni kayıt başarıyla eklendi.");
                }

                if (toUpdate.Any())
                {
                    await context.BulkUpdateAsync(toUpdate);
                    WatchLogger.Log($"{toUpdate.Count} adet kayıt başarıyla güncellendi.");
                }

                await context.SaveChangesAsync();

                var allEntitiesToSync = toInsert.Concat(toUpdate).ToList();
               

                var elasticTasks = allEntitiesToSync.Select(async entity =>
                {
                    try
                    {
                        await elasticSearchRepository.BulkAddToElasticSearchAsync(new List<TEntity> { entity });
                        WatchLogger.LogWarning($"Elasticsearch'e senkronize edildi. (Id: {GetId(entity) ?? "bilinmiyor"})");
                    }
                    catch (Exception esEx)
                    {
                        var json = JsonConvert.SerializeObject(entity);
                        var entityName = typeof(TEntity).Name;
                        rabbitMQProducer.Publish(json, entityName);
                        WatchLogger.LogWarning($"Elasticsearch'e senkronizasyon başarısız oldu. Veri RabbitMQ kuyruğuna gönderildi. Hata: {esEx.Message}");
                    }
                });

                await Task.WhenAll(elasticTasks);

                WatchLogger.Log("Toplu işlem başarıyla tamamlandı.");

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = $"{entities.Count} kayıt başarıyla işlendi.",
                    Data = entities
                };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($"Toplu işlem sırasında hata oluştu: {ex.Message}", ex.ToString());
                //throw new WriteBulkException("Toplu işlem sırasında beklenmeyen bir hata oluştu.", ex);
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Hata: {ex.Message}"
                };
            }
        }
        private static object? GetId(TEntity entity) => entity?.GetType().GetProperty("Id")?.GetValue(entity);

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
            WatchLogger.Log($" {typeof(TEntity).Name} için silme işlemi başlatıldı. Id: {id}");

            try
            {
                var entity = await this.Table.FindAsync(id);
                if (entity == null)
                {
                    WatchLogger.LogWarning($" Id: {id} olan {typeof(TEntity).Name} kaydı bulunamadı.");
                    return new BaseResponse
                    {
                        Succeeded = false,
                        Message = "Silinecek kayıt bulunamadı."
                    };
                }

                // Soft delete işlemi
                var isDeletedProp = entity.GetType().GetProperty("Isdeleted");
                if (isDeletedProp != null)
                {
                    isDeletedProp.SetValue(entity, true);
                    WatchLogger.Log($"Isdeleted özelliği true olarak ayarlandı. Id: {id}");
                }

                // UpdatedDate ayarı
                var updatedDateProp = entity.GetType().GetProperty("UpdatedAt");
                if (updatedDateProp != null)
                {
                    updatedDateProp.SetValue(entity, DateTime.UtcNow);
                    WatchLogger.Log($" UpdatedAt alanı UTC olarak güncellendi. Id: {id}");
                }

                this.context.Update(entity);
                await this.context.SaveChangesAsync();
                WatchLogger.Log($" {typeof(TEntity).Name} veritabanında soft delete ile güncellendi. Id: {id}");


                if (typeof(TEntity).Name.ToLower() == "customer")
                {
                    var redisRepo = serviceProvider.GetRequiredService<IRedisWriteRepository>();
                    var redisKey = $"customer:{id}";
                    await redisRepo.RemoveAsync(redisKey);
                    WatchLogger.Log($" Redis'ten silindi: {redisKey}");
                }

                try
                {
                    await this.elasticSearchRepository.BulkUpdateToElasticSearchAsync(new List<TEntity> { entity });
                    WatchLogger.Log($" {typeof(TEntity).Name} Elasticsearch üzerinde güncellendi. Id: {id}");
                }
                catch (Exception esEx)
                {
                    var jsonData = JsonConvert.SerializeObject(entity);
                    var entityName = typeof(TEntity).Name;
                    this.rabbitMQProducer.Publish(jsonData, entityName);
                    WatchLogger.LogWarning($" Elasticsearch güncelleme başarısız oldu. Veri RabbitMQ kuyruğuna gönderildi. Id: {id}, Hata: {esEx.Message}");
                }

                WatchLogger.Log($" Silme işlemi başarıyla tamamlandı. Id: {id}");

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Kayıt başarıyla silindi.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" Silme işlemi sırasında beklenmeyen bir hata oluştu. Id: {id}, Hata: {ex.Message}", ex.ToString());
                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Kayıt silinirken hata oluştu: {ex.Message}"
                };
            }
        }
        public async Task<BaseResponse> UpdateBulkAsync(ICollection<TEntity> entities)
        {
            WatchLogger.Log($" {typeof(TEntity).Name} için toplu güncelleme işlemi başlatıldı. Toplam kayıt: {entities.Count}");

            try
            {
                this.context.ChangeTracker.AutoDetectChangesEnabled = false;
                await this.context.BulkUpdateAsync(entities);
                this.context.ChangeTracker.AutoDetectChangesEnabled = true;

                WatchLogger.Log($" Veritabanında {entities.Count} kayıt başarıyla güncellendi.");

                try
                {
                    await this.elasticSearchRepository.BulkUpdateToElasticSearchAsync(entities);
                    WatchLogger.Log($" {entities.Count} kayıt Elasticsearch üzerinde de başarıyla güncellendi.");
                }
                catch (Exception esEx)
                {
                    WatchLogger.LogWarning($" Elasticsearch güncelleme işlemi başarısız oldu. RabbitMQ kuyruğuna aktarılıyor... Hata: {esEx.Message}");

                    foreach (var entity in entities)
                    {
                        var jsonData = JsonConvert.SerializeObject(entity);
                        var entityName = typeof(TEntity).Name;
                        this.rabbitMQProducer.Publish(jsonData, entityName);
                    }

                    WatchLogger.LogWarning($" {entities.Count} kayıt RabbitMQ kuyruğuna gönderildi.");
                }

                WatchLogger.Log($" Toplu güncelleme işlemi başarıyla tamamlandı.");

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = $"{entities.Count} kayıt başarıyla güncellendi.",
                    Data = entities
                };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" Toplu güncelleme sırasında beklenmeyen bir hata oluştu: {ex.Message}", ex.ToString());

                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Toplu kayıt güncellenirken hata oluştu: {ex.Message}"
                };
            }
        }
        public async Task<BaseResponse> UpdateSingleAsync(TEntity entity)
        {
            var entityName = typeof(TEntity).Name;
            var entityId = entity?.GetType().GetProperty("Id")?.GetValue(entity)?.ToString() ?? "belirsiz";

            WatchLogger.Log($" {entityName} için güncelleme işlemi başlatıldı. Id: {entityId}");

            try
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                context.Update(entity);
                context.ChangeTracker.AutoDetectChangesEnabled = true;

                WatchLogger.Log($" {entityName} veritabanında güncellendi. Id: {entityId}");

                try
                {
                    await this.elasticSearchRepository.SingleUpdateToElasticSearchAsync(entity);
                    WatchLogger.Log($" {entityName} Elasticsearch üzerinde güncellendi. Id: {entityId}");
                }
                catch (Exception esEx)
                {
                    var jsonData = JsonConvert.SerializeObject(entity);
                    this.rabbitMQProducer.Publish(jsonData, entityName);

                    WatchLogger.LogWarning($" Elasticsearch güncellemesi başarısız oldu. Veri RabbitMQ kuyruğuna gönderildi. Id: {entityId}, Hata: {esEx.Message}");
                }

                WatchLogger.Log($" Güncelleme işlemi başarıyla tamamlandı. Id: {entityId}");

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = $"Kayıt başarıyla güncellendi.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" Güncelleme sırasında beklenmeyen bir hata oluştu. Id: {entityId}, Hata: {ex.Message}", ex.ToString());

                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Kayıt güncellenirken hata oluştu: {ex.Message}"
                };
            }
        }
        public async Task<BaseResponse> RemoveAsync(List<TEntity> entities)
        {
            var entityName = typeof(TEntity).Name;
            WatchLogger.Log($" {entityName} için toplu silme işlemi başlatıldı. Silinecek kayıt sayısı: {entities.Count}");

            try
            {
                this.context.Set<TEntity>().RemoveRange(entities);
                await this.context.SaveChangesAsync();

                WatchLogger.Log($" {entityName} veritabanından {entities.Count} kayıt başarıyla silindi.");

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Kayıtlar başarıyla silindi"
                };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" {entityName} silme işlemi sırasında beklenmeyen bir hata oluştu: {ex.Message}", ex.ToString());

                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Silme sırasında hata: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse> RemoveRangeAsync(List<TEntity> entities)
        {
            var entityName = typeof(TEntity).Name;
            WatchLogger.Log($" {entityName} için toplu soft delete işlemi başlatıldı. Silinecek kayıt sayısı: {entities.Count}");

            try
            {
                foreach (var entity in entities)
                {
                    var isDeletedProp = entity.GetType().GetProperty("Isdeleted");
                    if (isDeletedProp != null)
                    {
                        isDeletedProp.SetValue(entity, true);
                    }

                    var updatedDateProp = entity.GetType().GetProperty("UpdatedAt");
                    if (updatedDateProp != null)
                    {
                        updatedDateProp.SetValue(entity, DateTime.UtcNow);
                    }
                }

                this.context.ChangeTracker.AutoDetectChangesEnabled = false;
                await this.context.BulkUpdateAsync(entities);
                this.context.ChangeTracker.AutoDetectChangesEnabled = true;

                WatchLogger.Log($" {entityName} veritabanında soft delete ile güncellendi. Kayıt sayısı: {entities.Count}");

                try
                {
                    await this.elasticSearchRepository.BulkUpdateToElasticSearchAsync(entities);
                    WatchLogger.Log($" {entityName} Elasticsearch üzerinde güncellendi. Kayıt sayısı: {entities.Count}");
                }
                catch (Exception esEx)
                {
                    WatchLogger.LogWarning($" Elasticsearch güncelleme başarısız oldu. RabbitMQ kuyruğuna aktarılıyor... Hata: {esEx.Message}");

                    foreach (var entity in entities)
                    {
                        var jsonData = JsonConvert.SerializeObject(entity);
                        this.rabbitMQProducer.Publish(jsonData, entityName);
                    }

                    WatchLogger.LogWarning($" {entities.Count} kayıt RabbitMQ kuyruğuna gönderildi.");
                }

                WatchLogger.Log($" Soft delete işlemi başarıyla tamamlandı.");

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Toplu silme işlemi başarıyla gerçekleştirildi.",
                    Data = entities
                };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" Soft delete işlemi sırasında beklenmeyen bir hata oluştu: {ex.Message}", ex.ToString());

                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Toplu silme işlemi sırasında hata oluştu: {ex.Message}"
                };
            }
        }
    }
}
