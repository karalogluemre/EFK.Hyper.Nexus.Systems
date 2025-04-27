using Commons.Application.Repositories.Commands;
using Commons.Application.Repositories.Common;
using Commons.Application.Repositories.Queries;
using Commons.Persistence.Repositories.Common;
using Commons.Persistence.Repositories.CrudRepositories.Commands;
using Commons.Persistence.Repositories.CrudRepositories.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Commons.Persistence.Injection
{
    public static class CommonDependencyInjections
    {
        public static void CommonDependencyInjectionService(this IServiceCollection services, IConfiguration configuration)
        {
            //Dependency Injection 
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            #region Db Context
            services.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>));
            services.AddScoped(typeof(IWriteRepository<,>), typeof(WriteRepository<,>));
            #endregion

            #region Unit Of Work
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            #endregion

            #region ElasticSearch
            services.AddScoped(typeof(IElasticSearchReadRepository<,>), typeof(ElasticSearchReadRepository<,>));
            services.AddScoped(typeof(IElasticSearchWriteRepository<,>), typeof(ElasticSearchWriteRepository<,>));
            #endregion

            #region Rabbit MQ
            services.AddScoped<IRabbitMQProducer,RabbitMQProducer>();
            #endregion

            #region Mongo 
            services.AddScoped<IMongoWriteRepository, MongoWriteRepository>();
            services.AddScoped(typeof(IMongoReadRepository<>), typeof(MongoReadRepository<>));
            #endregion

        }
    }
}
