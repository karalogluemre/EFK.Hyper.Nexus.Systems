using Commons.Persistence.Configurations.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Commons.Persistence.Registrations
{
    public static class AutoMapperRegistration
    {
        public static void ApplyAllConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
            services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
        }
    }
}
