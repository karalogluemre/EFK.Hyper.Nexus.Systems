using Commons.Persistence.Mapper.Adreses;
using Commons.Persistence.Mapper.Branch;
using Commons.Persistence.Mapper.Company;
using Commons.Persistence.Mapper.Package;
using Commons.Persistence.Mapper.Role;
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
            services.AddAutoMapper(typeof(RoleMappingProfile).Assembly);
            services.AddAutoMapper(typeof(PackageMappingProfile).Assembly);
            services.AddAutoMapper(typeof(CompanyMappingProfile).Assembly);
            services.AddAutoMapper(typeof(AdresesMappingProfile).Assembly);
            services.AddAutoMapper(typeof(BranchMappingProfile).Assembly);
        }
    }
}
