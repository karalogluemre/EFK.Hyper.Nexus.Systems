using Insure.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EFK.System.Persistence.Injection.CDC
{
    public static class CDCDependencyInjection
    {
        public static void CdcDependencyInjectionService(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddHostedService<CDCReaderService<ApplicationDbContext,Commons.Domain.Models.User.AppUser>>();
        }
    }
}
    