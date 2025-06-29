using Commons.Application.Extension;
using Commons.Application.Repositories.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.Common;
using WatchDog;

namespace EFK.System.Persistence.Injection.CDC
{
    public class CDCReaderService<TDbContext, TEntity>(
  IServiceProvider serviceProvider
) : BackgroundService
  where TDbContext : DbContext
  where TEntity : class
    {
        private readonly IServiceProvider serviceProvider = serviceProvider; // IServiceProvider üzerinden gerekli servisleri alabilmek için 
        private readonly string cdcTableName = $"cdc.dbo_{typeof(TEntity).Name.ToLower()}_CT"; // Change Data Capture (CDC) tablosunun adını belirledim
        private readonly string baseTableName = typeof(TEntity).Name; // Temel tablonun adını belirledim

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            WatchLogger.Log($"{baseTableName} CDC Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // Yeni scope, dbContext ve elasticWriter her döngüde oluşturulur
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                var elasticWriter = scope.ServiceProvider.GetRequiredService<IElasticSearchWriteRepository<TDbContext, TEntity>>();

                try
                {
                    await dbContext.Database.OpenConnectionAsync();
                    using var command = dbContext.Database.GetDbConnection().CreateCommand();
                    command.CommandText = $"SELECT * FROM {cdcTableName}";

                    using var reader = await command.ExecuteReaderAsync(stoppingToken);

                    bool hasData = false;
                    //CDC'den kayıtları aldı ve elastik ile eşleştirdi db'yi
                    while (await reader.ReadAsync(stoppingToken))
                    {
                        hasData = true;

                        var operation = Convert.ToInt32(reader["__$operation"]);
                        var entity = MapReaderToEntity(reader);

                        switch (operation)
                        {
                            case 1:
                                await elasticWriter.BulkDeleteFromElasticSearchAsync(new List<TEntity> { entity });
                                break;
                            case 2:
                                await elasticWriter.BulkAddToElasticSearchAsync(new List<TEntity> { entity });
                                break;
                            case 4:
                                await elasticWriter.BulkUpdateToElasticSearchAsync(new List<TEntity> { entity });
                                break;
                        }
                    }


                    if (hasData)
                    {
                        //Eğer kayıt varsa ilgili kayıtları gidip elasticteki log tablosuna ekleyecek ardından cdc tablosunu temizleyecek
                        //Database'den manuel işlem yapıldığında hangi kullanıcı işlemi yapmış hangi tarih hangi ip adresi gibi bilgileri tutacak şekilde ayarlanması lazım. cdc tablosuna kayıt geldiği zaman trigger yardımı ile bu bilgiler logta tutulabilir.
                        //Step 1: Cdc'deki kayıtları bul
                        //Step 2: ElasticSearch'e kayıtları ekle
                        //Step 3: Log kayıtlarını elasticteki log tablosuna ekle (userid, ip address ,username, islem tarihi bilgileriyle)
                        //Step 4: CDC tablosunu temizle
                        await reader.DisposeAsync(); // reader kapandı
                        using var truncateCommand = dbContext.Database.GetDbConnection().CreateCommand();
                        truncateCommand.CommandText = $"TRUNCATE TABLE {cdcTableName}";
                        await truncateCommand.ExecuteNonQueryAsync(stoppingToken);
                    }

                }
                catch (Exception ex)
                {
                    WatchLogger.Log(ex.ToString(), $"Hata {baseTableName} CDC service.");
                }
                finally
                {
                    await dbContext.Database.CloseConnectionAsync();
                }
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }

        private TEntity MapReaderToEntity(DbDataReader reader)
        {
            var entity = Activator.CreateInstance<TEntity>();
            foreach (var prop in typeof(TEntity).GetProperties())
            {
                if (!reader.HasColumn(prop.Name) || reader[prop.Name] is DBNull) continue;
                prop.SetValue(entity, reader[prop.Name]);
            }
            return entity;
        }
    }
}
