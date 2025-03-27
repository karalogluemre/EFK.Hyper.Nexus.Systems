using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WatchDog;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class SyncElasticsearchService<TDbContext, TEntity> : BackgroundService
       where TDbContext : DbContext
       where TEntity : class
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SyncElasticsearchService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
            var elasticSearchReadRepository = scope.ServiceProvider.GetRequiredService<IElasticSearchReadRepository<TDbContext, TEntity>>();
            var elasticSearchRepository = scope.ServiceProvider.GetRequiredService<IElasticSearchWriteRepository<TDbContext, TEntity>>();

            try
            {
                WatchLogger.Log($" SyncElasticsearchService {typeof(TEntity).Name} başlatıldı...");

                WatchLogger.Log("MSSQL ile Elasticsearch senkronizasyonu başlatılıyor...");

                // MSSQL'deki tüm verileri al
                var sqlData = await context.Set<TEntity>().ToListAsync(stoppingToken);
                if (!sqlData.Any())
                {
                    WatchLogger.Log($" {typeof(TEntity).Name} tablosunda MSSQL'de hiç veri bulunamadı!");
                }
                foreach (var sqlEntity in sqlData)
                {
                    WatchLogger.Log($"MSSQL: {typeof(TEntity).Name} - Id: {GetId(sqlEntity)}");
                }
                // Elasticsearch'teki tüm verileri al
                var esData = await elasticSearchReadRepository.GetAllFromElasticSearchAsync();
                foreach (var esEntity in esData)
                {
                    WatchLogger.Log($" Elasticsearch: {typeof(TEntity).Name} - Id: {GetId(esEntity)}");
                }
                // MSSQL’de olup Elasticsearch’te olmayanları bul
                var missingInElastic = sqlData.Where(x => !esData.Any(y => GetId(y) == GetId(x))).ToList();

                if (missingInElastic.Any())
                {
                    WatchLogger.Log($"{missingInElastic.Count} {typeof(TEntity).Name} kayıt Elasticsearch'e ekleniyor...");
                    await elasticSearchRepository.BulkAddToElasticSearchAsync(missingInElastic);
                }

                // **Elasticsearch'te olup MSSQL’de olmayanları bul ve sil**

                var extraInElastic = esData.Where(x =>!sqlData.Any(y => GetId(y) == GetId(x))).ToList();
                if (extraInElastic.Any())
                {
                    WatchLogger.LogWarning($"{extraInElastic.Count} {typeof(TEntity).Name} kayıt Elasticsearch'te fazla, siliyoruz...");
                    foreach (var deletedEntity in extraInElastic)
                    {
                        WatchLogger.LogWarning($" Silinen Kayıt: {typeof(TEntity).Name} - Id: {GetId(deletedEntity)}");
                    }
                    await elasticSearchRepository.BulkDeleteFromElasticSearchAsync(extraInElastic);
                }

                WatchLogger.Log($" {typeof(TEntity).Name} Veritabanı senkronizasyonu tamamlandı.");
            }
            catch (Exception ex)
            {
                WatchLogger.LogError($" {typeof(TEntity).Name} Senkranizasyon sırasında hata oluştu: {ex.Message}");
            }
        }
        private static Guid GetId(object entity)
        {
            var idValue = entity.GetType().GetProperty("Id")?.GetValue(entity, null);
            return idValue != null ? Guid.Parse(idValue.ToString()) : Guid.Empty;
        }

    }

}
