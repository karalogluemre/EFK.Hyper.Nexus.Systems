using Commons.Application.Repositories.Commands;
using Commons.Domain.Models.User;
using Commons.Persistence.Repositories.CrudRepositories.Commands;
using Insure.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Insure.Persistence.Injection
{

    public static class RabbitElasticDependencyInjection
    {
        public static void RabbitMQElasticSearchDependencyInjectionService(this IServiceCollection services, IConfiguration configuration)
        {
            //her yeni projenin modellerinin farklı olmasına adına context'i burada tanımladım. 
            #region Rabbit MQ
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, AppUser>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, AppRole>>();
            #endregion

            #region ElasticSearch
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, AppUser>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, AppRole>>(); 
            #endregion
        }
    }
}
