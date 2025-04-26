using Commons.Application.Abstract.Authorization;
using Commons.Domain.Message;
using Commons.Domain.Models.Role;
using Commons.Domain.Models.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class IdentityRegistration
{
    public static void AddIdentityRegistration<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            // Şifre Politikaları
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;

            // Kilitleme Politikaları
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Kullanıcı Adı ve Email Politikaları
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+üğışçöÜĞİŞÇÖ";
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<AppRole>()
        .AddEntityFrameworkStores<TContext>()
        .AddDefaultTokenProviders()
        .AddErrorDescriber<CustomIdentityErrorDescriber>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Menus", policy =>
                policy.Requirements.Add(new MenuAccessRequirement()));
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ClockSkew = TimeSpan.Zero 
            };
        });

        services.AddScoped<UserManager<AppUser>>();
        services.AddScoped<SignInManager<AppUser>>();
        services.AddScoped<RoleManager<AppRole>>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });

        services.AddControllers();
    }
}
