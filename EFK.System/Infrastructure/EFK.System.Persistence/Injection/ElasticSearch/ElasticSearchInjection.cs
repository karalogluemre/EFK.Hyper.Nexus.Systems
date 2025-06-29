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

namespace EFK.System.Persistence.Injection.ElasticSearch
{
    public static class ElasticSearchInjection
    {
        public static void ElasticSearchDependencyInjectionService(this IServiceCollection services, IConfiguration configuration)
        {
            #region ElasticSearch
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, AppUser>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, AppRole>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Menu>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Package>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, PackageMenu>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Company>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, District>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Province>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, CompanyFile>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, FileMetaData>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, Branch>>();
            services.AddHostedService<SyncElasticsearchService<ApplicationDbContext, BranchMenu>>();
            #endregion
        }

    }
}