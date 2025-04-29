using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using WatchDog;
using WatchDog.src.Enums;

namespace Commons.Persistence.Registrations
{
    public static class ServicesRegistration
    {
        public static void AddPersistenceServices<TContext>(this IServiceCollection services, IConfiguration configuration, string connectionStringName, string log)
       where TContext : DbContext
        {
            #region DbContext
            services.AddDbContext<TContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString(connectionStringName));
            });
            #endregion

            #region Log

            services.AddWatchDogServices(opt =>
            {
                opt.IsAutoClear = true;
                opt.SetExternalDbConnString = configuration.GetConnectionString(log);
                opt.DbDriverOption = WatchDogDbDriverEnum.MSSQL;
            });

            #endregion

            #region Mongo 
            services.AddSingleton<IMongoClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return new MongoClient(config["MongoSettings:ConnectionString"]);
            });

            services.AddScoped(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var client = sp.GetRequiredService<IMongoClient>();
                var database = client.GetDatabase(config["MongoSettings:DatabaseName"]);
                return new GridFSBucket(database);
            });

            #endregion

            #region Redis

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{configuration["RedisSettings:Host"]}:{configuration["RedisSettings:Port"]}";
                options.InstanceName = configuration["RedisSettings:InstanceName"];
            });
            #endregion




            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader();
                });
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.MaxDepth = 64;
            });

        }
    }
}
