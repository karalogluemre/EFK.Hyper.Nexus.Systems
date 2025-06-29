using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WatchDog;

namespace Commons.Persistence.Repositories.CrudRepositories.Commands
{
    public class SyncElasticsearchService<TDbContext, TEntity>(
        IServiceScopeFactory serviceScopeFactory
        ) : BackgroundService
       where TDbContext : DbContext
       where TEntity : class
    {
        private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            {
                using var scope = this.serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
                var elasticSearchReadRepository = scope.ServiceProvider.GetRequiredService<Application.Repositories.Queries.IElasticSearchReadRepository<TDbContext, TEntity>>();
                var elasticSearchRepository = scope.ServiceProvider.GetRequiredService<Application.Repositories.Commands.IElasticSearchWriteRepository<TDbContext, TEntity>>();

                try
                {
                    WatchLogger.Log($" SyncElasticsearchService {typeof(TEntity).Name} başlatıldı...");


                    var sqlData = await context.Set<TEntity>().ToListAsync(stoppingToken);

                    var esData = await elasticSearchReadRepository.GetAllFromElasticSearchAsync();

                    var missingInElastic = sqlData.Where(x => !esData.Any(y => GetId(y) == GetId(x))).ToList();


                    await Parallel.ForEachAsync(
                                                    missingInElastic.Chunk(5000),
                                                    new ParallelOptions { MaxDegreeOfParallelism = 4 },
                                                    async (chunk, _) =>
                                                    {
                                                        await elasticSearchRepository.BulkAddToElasticSearchAsync(chunk);
                                                    });

                    var extraInElastic = esData.Where(x => !sqlData.Any(y => GetId(y) == GetId(x))).ToList();
                    if (extraInElastic.Any())
                    {
                        WatchLogger.LogWarning($"{extraInElastic.Count} {typeof(TEntity).Name} kayıt Elasticsearch'te fazla, siliyoruz...");
                        await elasticSearchRepository.BulkDeleteFromElasticSearchAsync(extraInElastic);
                    }

                    WatchLogger.Log($" {typeof(TEntity).Name} Veritabanı senkronizasyonu tamamlandı.");
                }
                catch (Exception ex)
                {
                    WatchLogger.LogError($" {typeof(TEntity).Name} Senkranizasyon sırasında hata oluştu: {ex.Message}");
                }
            }
        }
        private static int GetId(object entity)
        {
            var idValue = entity.GetType().GetProperty("Id")?.GetValue(entity, null);
            return idValue != null ? Convert.ToInt32(idValue) : 0;
        }
    }
}
