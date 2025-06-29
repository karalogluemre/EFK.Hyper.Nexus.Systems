using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using EFK.Commons.Config;
using Microsoft.EntityFrameworkCore;
using Nest;
using WatchDog;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class ElasticSearchWriteRepository<TContext, TEntity> : IElasticSearchWriteRepository<TContext, TEntity>
         where TContext : DbContext
         where TEntity : class
    {
        private readonly ElasticClient elasticClient;
        public ElasticSearchWriteRepository()
        {
            try
            {
                var configInitializer = new AppSettingsConfiguration();
                var appSecrets = configInitializer.InitializeConfigsAsync().GetAwaiter().GetResult();
                var url = appSecrets.ElasticSearchSettings.Url;
                if (string.IsNullOrWhiteSpace(url))
                {
                    WatchLogger.LogWarning("[ElasticSearchWriteRepository] ElasticSearch:Url değeri yapılandırmada bulunamadı veya boş.");
                }

                var settings = new ConnectionSettings(new Uri(url))
                    .DisableDirectStreaming()
                    .DefaultMappingFor<TEntity>(m => m.IndexName(typeof(TEntity).Name.ToLower()));

                this.elasticClient = new ElasticClient(settings);

                WatchLogger.Log($"[ElasticSearchWriteRepository] ElasticSearch bağlantısı başarıyla kuruldu. URL: {url}, Index: {typeof(TEntity).Name.ToLower()}");
            }
            catch (Exception ex)
            {
                WatchLogger.LogError("[ElasticSearchWriteRepository] ElasticSearch istemcisi oluşturulurken hata oluştu: " + ex.Message, ex.ToString());
                throw; // önemli: dışa at, aksi halde silent fail olabilir
            }
        }

        public async Task<BaseResponse> AddToElasticSearchAsync(TEntity entity)
        {
            try
            {
                var response = await this.elasticClient.IndexDocumentAsync(entity);
                if (!response.IsValid)
                {
                    var errorMessage = $"Elasticsearch'e veri yazma başarısız. Hata: {response.OriginalException?.Message}";
                    WatchLogger.LogError(errorMessage);
                    return new BaseResponse { Succeeded = false, Message = errorMessage };
                }

                return new BaseResponse { Succeeded = true, Message = "Kayıt Elasticsearch'e başarıyla eklendi.", Data = entity };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($"Elasticsearch'e veri eklenirken hata oluştu: {ex.Message}");
                return new BaseResponse { Succeeded = false, Message = $"Elasticsearch'e veri eklenirken hata oluştu: {ex.Message}" };
            }
        }

        public async Task<BaseResponse> BulkAddToElasticSearchAsync(IEnumerable<TEntity> data)
        {
            var indexName = typeof(TEntity).Name.ToLower();
            var count = data.Count();

            WatchLogger.Log($" {count} adet {typeof(TEntity).Name} verisi Elasticsearch'e eklenmek üzere hazırlanıyor.");

            try
            {
                var bulkDescriptor = new BulkDescriptor();

                foreach (var entity in data)
                {
                    bulkDescriptor.Index<TEntity>(op => op
                        .Index(indexName)
                        .Id(new Id(entity))
                        .Document(entity)
                    );
                }

                var response = await this.elasticClient.BulkAsync(bulkDescriptor);
                await Task.Delay(500); // Elasticsearch gecikme önlemi

                if (response.Errors)
                {
                    WatchLogger.LogError(" Elasticsearch toplu ekleme işlemi sırasında hatalar oluştu:");

                    foreach (var item in response.ItemsWithErrors)
                    {
                        WatchLogger.LogError($" - Hata: {item.Error.Reason}");
                    }

                    return new BaseResponse
                    {
                        Succeeded = false,
                        Message = "Elasticsearch Bulk işlemi sırasında hata oluştu!"
                    };
                }

                WatchLogger.Log($" {count} adet {typeof(TEntity).Name} kaydı Elasticsearch'e başarıyla eklendi. Index: {indexName}");

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = $"{count} kayıt Elasticsearch'e başarıyla eklendi.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" Elasticsearch'e toplu ekleme sırasında beklenmeyen bir hata oluştu: {ex.Message}", ex.ToString());

                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Elasticsearch Bulk ekleme sırasında hata oluştu: {ex.Message}"
                };
            }
        }
        public async Task<BaseResponse> BulkUpdateToElasticSearchAsync(IEnumerable<TEntity> data)
        {
            try
            {
                var bulkDescriptor = new BulkDescriptor();

                foreach (var entity in data)
                {
                    bulkDescriptor.Update<TEntity>(op => op
                        .Index(typeof(TEntity).Name.ToLower()) // İlgili index
                        .Id(new Id(entity)) // ID'yi kullanarak güncelleme yap
                        .Doc(entity) // Güncellenecek veri
                        .DocAsUpsert(true) // Eğer yoksa ekle
                    );
                }

                var response = await this.elasticClient.BulkAsync(bulkDescriptor);
                await Task.Delay(500);

                if (response.Errors)
                {
                    WatchLogger.LogError("Elasticsearch Bulk Güncelleme Hata Detayları:");
                    foreach (var item in response.ItemsWithErrors)
                    {
                        WatchLogger.LogError($" Hata: {item.Error.Reason}");
                    }

                    return new BaseResponse { Succeeded = false, Message = "Elasticsearch Bulk Güncelleme işlemi sırasında hata oluştu!" };
                }

                return new BaseResponse { Succeeded = true, Message = $"{data.Count()} kayıt Elasticsearch'te başarıyla güncellendi.", Data = data };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($"Elasticsearch Bulk Güncelleme sırasında hata oluştu: {ex.Message}");
                return new BaseResponse { Succeeded = false, Message = $"Elasticsearch Bulk Güncelleme sırasında hata oluştu: {ex.Message}" };
            }
        }
        public async Task<BaseResponse> SingleUpdateToElasticSearchAsync(TEntity entity)
        {
            var indexName = typeof(TEntity).Name.ToLower();
            var entityId = entity?.GetType().GetProperty("Id")?.GetValue(entity)?.ToString() ?? "belirsiz";

            WatchLogger.Log($" {indexName} index'inde Id {entityId} olan kayıt güncellenmek üzere hazırlanıyor.");

            try
            {
                var bulkDescriptor = new BulkDescriptor();

                bulkDescriptor.Update<TEntity>(op => op
                    .Index(indexName)
                    .Id(new Id(entity))
                    .Doc(entity)
                    .DocAsUpsert(true)
                );

                var response = await this.elasticClient.BulkAsync(bulkDescriptor);
                await Task.Delay(500); // Gecikme, işlem tamamlanmadan döndürmemek için

                if (response.Errors)
                {
                    WatchLogger.LogError(" Elasticsearch güncelleme sırasında hata oluştu:");
                    foreach (var item in response.ItemsWithErrors)
                    {
                        WatchLogger.LogError($" - Hata: {item.Error.Reason}");
                    }

                    return new BaseResponse
                    {
                        Succeeded = false,
                        Message = "Elasticsearch güncelleme işlemi sırasında hata oluştu!"
                    };
                }

                WatchLogger.Log($" {indexName} index'inde Id {entityId} olan kayıt başarıyla güncellendi.");

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = "Kayıt Elasticsearch'te başarıyla güncellendi.",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" Elasticsearch güncelleme sırasında beklenmeyen bir hata oluştu. Id: {entityId}, Hata: {ex.Message}", ex.ToString());

                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Elasticsearch güncelleme sırasında hata oluştu: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse> BulkDeleteFromElasticSearchAsync(IEnumerable<TEntity> data)
        {
            var indexName = typeof(TEntity).Name.ToLower();
            var count = data.Count();

            WatchLogger.Log($" Elasticsearch'ten {count} adet {typeof(TEntity).Name} kaydı silinmek üzere hazırlanıyor. Index: {indexName}");

            try
            {
                var bulkDescriptor = new BulkDescriptor();

                foreach (var entity in data)
                {
                    bulkDescriptor.Delete<TEntity>(op => op
                        .Index(indexName)
                        .Id(new Id(entity))
                    );
                }

                var response = await this.elasticClient.BulkAsync(bulkDescriptor);

                if (response.Errors)
                {
                    WatchLogger.LogError(" Elasticsearch toplu silme işlemi sırasında hatalar oluştu:");

                    foreach (var item in response.ItemsWithErrors)
                    {
                        WatchLogger.LogError($" - Hata: {item.Error.Reason}");
                    }

                    return new BaseResponse
                    {
                        Succeeded = false,
                        Message = "Elasticsearch Bulk Silme işlemi sırasında hata oluştu!"
                    };
                }

                WatchLogger.Log($" {count} kayıt Elasticsearch'ten başarıyla silindi. Index: {indexName}");

                return new BaseResponse
                {
                    Succeeded = true,
                    Message = $"{count} kayıt Elasticsearch'ten başarıyla silindi."
                };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" Elasticsearch toplu silme sırasında beklenmeyen bir hata oluştu: {ex.Message}", ex.ToString());

                return new BaseResponse
                {
                    Succeeded = false,
                    Message = $"Elasticsearch Bulk silme sırasında hata oluştu: {ex.Message}"
                };
            }
        }
    }
}