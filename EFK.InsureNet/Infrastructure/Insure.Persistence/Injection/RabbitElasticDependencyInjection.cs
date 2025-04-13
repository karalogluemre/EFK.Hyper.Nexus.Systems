using Commons.Domain.Models.Role;
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
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Commons.Domain.Models.Menus.Menu>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Commons.Domain.Models.Packages.Package>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Commons.Domain.Models.Packages.PackageMenu>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Commons.Domain.Models.Companies.Company>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Commons.Domain.Models.Adreses.District>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Commons.Domain.Models.Adreses.Province>>();
            #endregion

            #region ElasticSearch
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, AppUser>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, AppRole>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Commons.Domain.Models.Menus.Menu>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Commons.Domain.Models.Packages.Package>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Commons.Domain.Models.Packages.PackageMenu>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Commons.Domain.Models.Companies.Company>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Commons.Domain.Models.Adreses.District>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Commons.Domain.Models.Adreses.Province>>();
            #endregion

        }
    }
}
