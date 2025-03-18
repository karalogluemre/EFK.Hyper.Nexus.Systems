using Commons.Application.Features.Commands.Menu.Create;
using Commons.Application.Features.Commands.Role.Create;
using Commons.Application.Features.Commands.User.Create;
using Commons.Application.Features.Queries.Menu;
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

            #region Role
            services.AddScoped<IRequestHandler<CreateRoleCommandRequest, BaseResponse>, CreateRoleCommandHandler<ApplicationDbContext>>();
            services.AddScoped<IRequestHandler<UpdateRoleCommandRequest, BaseResponse>, UpdateRoleCommandHandler<ApplicationDbContext>>();

            #endregion

            #region Menu
            services.AddScoped<IRequestHandler<CreateMenuCommandRequest, BaseResponse>, CreateMenuCommandHandler<ApplicationDbContext>>();
            services.AddScoped<IRequestHandler<GetAllMenuQueryRequest, List<Commons.Domain.Models.Menus.Menu>>, GetAllMenuQueryHandler<ApplicationDbContext>>();
            #endregion
        }
    }
}
