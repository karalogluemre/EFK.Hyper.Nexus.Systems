using Commons.Application.Repositories.Commands;
using Commons.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nest;

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

        public async Task AddToElasticSearchAsync(TEntity entity)
        {
            var response = await this.elasticClient.IndexDocumentAsync(entity);
            if (!response.IsValid)
            {
                throw new Exception($"Elasticsearch'e veri yazma başarısız. Hata: {response.OriginalException.Message}");
            }
        }
        public async Task BulkAddToElasticSearchAsync(IEnumerable<TEntity> data)
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

            // Eğer response'da "errors": false ise, hata yok demektir.
            if (response.Errors)
            {
                Console.WriteLine("Elasticsearch Bulk Hata Detayları:");
                foreach (var item in response.ItemsWithErrors)
                {
                    Console.WriteLine($"Hata: {item.Error.Reason}");
                }

                throw new Exception("Elasticsearch Bulk işlemi sırasında hata oluştu!");
            }
        }

        public async Task BulkDeleteFromElasticSearchAsync(IEnumerable<TEntity> data)
        {
            var bulkDescriptor = new BulkDescriptor();

            foreach (var entity in data)
            {
                bulkDescriptor.Delete<TEntity>(op => op
                    .Index(typeof(TEntity).Name.ToLower()) // Doğru index ismi
                    .Id(new Id(entity)) // Silinecek ID'yi belirle
                );
            }

            var response = await this.elasticClient.BulkAsync(bulkDescriptor);

            // Eğer response'da "errors": false ise, hata yok demektir.
            if (response.Errors)
            {
                Console.WriteLine("Elasticsearch Bulk Silme Hata Detayları:");
                foreach (var item in response.ItemsWithErrors)
                {
                    Console.WriteLine($"Hata: {item.Error.Reason}");
                }

                throw new Exception("Elasticsearch Bulk Silme işlemi sırasında hata oluştu!");
            }
            else
            {
                Console.WriteLine($"Elasticsearch'ten {data.Count()} kayıt başarıyla silindi.");
            }
        }


    }
}
