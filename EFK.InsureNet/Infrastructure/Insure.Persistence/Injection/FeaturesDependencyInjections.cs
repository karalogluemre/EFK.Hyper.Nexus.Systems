using Commons.Application.Features.Commands.Menu.Create;
using Commons.Application.Features.Commands.Package.Create;
using Commons.Application.Features.Commands.Package.Remove;
using Commons.Application.Features.Commands.Package.Update;
using Commons.Application.Features.Commands.Role.Create;
using Commons.Application.Features.Commands.Role.Update;
using Commons.Application.Features.Commands.User.Create;
using Commons.Application.Features.Queries.Menu;
using Commons.Application.Features.Queries.Package;
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

            #region Package
            services.AddScoped<IRequestHandler<CreatePackageCommandRequest, BaseResponse>, CreatePackageCommandHandler<ApplicationDbContext>>();
            services.AddScoped<IRequestHandler<UpdatePackageCommandRequest, BaseResponse>, UpdatePackageCommandHandler<ApplicationDbContext>>();
            services.AddScoped<IRequestHandler<DeletePackageCommandRequest, BaseResponse>, DeletePackageCommandHandler<ApplicationDbContext>>();
            services.AddScoped<IRequestHandler<DeleteBulkAllPackageCommandRequest, BaseResponse>, DeleteBulkAllPackageCommandHandler<ApplicationDbContext>>();
            services.AddScoped<IRequestHandler<GetAllPackagesQueryRequest, List<Commons.Domain.Models.Packages.Package>>, GetAllPackagesQueryHandler<ApplicationDbContext>>();
            services.AddScoped<IRequestHandler<GetPackageByIdQueryRequest, Commons.Domain.Models.Packages.Package>, GetPackageByIdQueryHandler<ApplicationDbContext>>();
            #endregion
        }
    }
}
