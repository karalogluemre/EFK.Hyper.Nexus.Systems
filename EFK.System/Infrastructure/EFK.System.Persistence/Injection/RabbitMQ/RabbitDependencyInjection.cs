using Commons.Domain.Models.Adreses;
using Commons.Domain.Models.Branches;
using Commons.Domain.Models.Companies;
using Commons.Domain.Models.Menus;
using Commons.Domain.Models.Packages;
using Commons.Domain.Models.Role;
using Commons.Domain.Models.User;
using Commons.Domain.MongoFile;
using Commons.Persistence.Repositories.CrudRepositories.Commands;
using Insure.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EFK.System.Persistence.Injection.RabbitMQ
{

    public static class RabbitDependencyInjection
    {
        public static void RabbitMQDependencyInjectionService(this IServiceCollection services, IConfiguration configuration)
        {
            //her yeni projenin modellerinin farklı olmasına adına context'i burada tanımladım. 
            #region Rabbit MQ
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, AppUser>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, AppRole>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Menu>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Package>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, PackageMenu>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Company>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, District>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Province>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, CompanyFile>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, Branch>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, BranchMenu>>();
            services.AddHostedService<RabbitMQConsumerService<ApplicationDbContext, FileMetaData>>();
            #endregion

        }
    }
}