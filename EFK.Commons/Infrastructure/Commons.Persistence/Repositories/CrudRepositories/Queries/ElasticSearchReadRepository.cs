using Commons.Application.Repositories.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nest;

namespace Commons.Persistence.Repositories.CrudRepositories.Queries
{
    public class ElasticSearchReadRepository<TContext, TEntity> : IElasticSearchReadRepository<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        private readonly ElasticClient elasticClient;
        private readonly TContext context;

        public ElasticSearchReadRepository(IConfiguration configuration, TContext context)
        {
            this.context = context;
            var settings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Url"]))
                 .DisableDirectStreaming()
                                .DefaultMappingFor<TEntity>(m => m
                                .IndexName(typeof(TEntity).Name.ToLower())
        );
            elasticClient = new ElasticClient(settings);
        }
        public async Task<List<TEntity>> GetAllFromElasticSearchAsync()
        {
            var response = await this.elasticClient.SearchAsync<TEntity>(s => s
                .Index(typeof(TEntity).Name.ToLower())
                .Query(q => q.MatchAll())
                .Size(10000)
            );

            return response.Documents.ToList();
        }
    }

}
