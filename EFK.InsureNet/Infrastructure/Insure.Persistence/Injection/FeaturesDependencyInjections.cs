using Commons.Application.Features.Commands.User.Create;
using Commons.Application.Features.Queries.User;
using Commons.Application.Token;
using Commons.Domain.Models;
using Insure.Persistence.Context;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace Insure.Persistence.Injection
{
    public static class FeaturesDependencyInjections
    {
        public static void FeaturesDependencyInjectionServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            #region Users
            services.AddScoped<GenerateJwtToken>();
            services.AddScoped<IRequestHandler<LoginUserQueryRequest, BaseResponse>, LoginUserQueryHandler<ApplicationDbContext>>();
            services.AddScoped<IRequestHandler<RegisterUserCommandRequest, BaseResponse>, RegisterUserCommandHandler<ApplicationDbContext>>();
            #endregion
        }
    }
}
