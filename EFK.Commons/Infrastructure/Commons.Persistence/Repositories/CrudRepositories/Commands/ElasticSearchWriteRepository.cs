using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nest;
using WatchDog;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class ElasticSearchWriteRepository<TContext, TEntity> : IElasticSearchWriteRepository<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        private readonly ElasticClient elasticClient;
        private readonly TContext context;

        public ElasticSearchWriteRepository(IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Url"]))
                  .DisableDirectStreaming() 
                  .DefaultMappingFor<TEntity>(m => m.IndexName(typeof(TEntity).Name.ToLower()));
           this.elasticClient = new ElasticClient(settings);
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
            try
            {
                var bulkDescriptor = new BulkDescriptor();

                foreach (var entity in data)
                {
                    bulkDescriptor.Index<TEntity>(op => op
                        .Index(typeof(TEntity).Name.ToLower())
                        .Id(new Id(entity))
                        .Document(entity)
                    );
                }

                var response = await this.elasticClient.BulkAsync(bulkDescriptor);

                if (response.Errors)
                {
                    WatchLogger.LogError("Elasticsearch Bulk Hata Detayları:");
                    foreach (var item in response.ItemsWithErrors)
                    {
                        WatchLogger.LogError($" Hata: {item.Error.Reason}");
                    }

                    return new BaseResponse { Succeeded = false, Message = "Elasticsearch Bulk işlemi sırasında hata oluştu!" };
                }

                return new BaseResponse { Succeeded = true, Message = $"{data.Count()} kayıt Elasticsearch'e başarıyla eklendi.", Data = data };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($"Elasticsearch Bulk ekleme sırasında hata oluştu: {ex.Message}");
                return new BaseResponse { Succeeded = false, Message = $"Elasticsearch Bulk ekleme sırasında hata oluştu: {ex.Message}" };
            }
        }

        public async Task<BaseResponse> BulkDeleteFromElasticSearchAsync(IEnumerable<TEntity> data)
        {
            try
            {
                var bulkDescriptor = new BulkDescriptor();

                foreach (var entity in data)
                {
                    bulkDescriptor.Delete<TEntity>(op => op
                        .Index(typeof(TEntity).Name.ToLower())
                        .Id(new Id(entity))
                    );
                }

                var response = await this.elasticClient.BulkAsync(bulkDescriptor);

                if (response.Errors)
                {
                    WatchLogger.LogError("Elasticsearch Bulk Silme Hata Detayları:");
                    foreach (var item in response.ItemsWithErrors)
                    {
                        WatchLogger.LogError($" Hata: {item.Error.Reason}");
                    }

                    return new BaseResponse { Succeeded = false, Message = "Elasticsearch Bulk Silme işlemi sırasında hata oluştu!" };
                }

                WatchLogger.Log($" Elasticsearch'ten {data.Count()} kayıt başarıyla silindi.");
                return new BaseResponse { Succeeded = true, Message = $"{data.Count()} kayıt Elasticsearch'ten başarıyla silindi." };
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($"Elasticsearch Bulk silme sırasında hata oluştu: {ex.Message}");
                return new BaseResponse { Succeeded = false, Message = $"Elasticsearch Bulk silme sırasında hata oluştu: {ex.Message}" };
            }
        }
    }
}
