using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


namespace Commons.Persistence.Registrations
{
    public static class SwaggerRegistration
    {
        public static void SwaggerGenRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = configuration["SwaggerSettings:Title"],
                    Version = configuration["SwaggerSettings:Version"],
                    Description = configuration["SwaggerSettings:Description"],
                    Contact = new OpenApiContact
                    {
                        Name = configuration["SwaggerSettings:Contact:Name"],
                        Email = configuration["SwaggerSettings:Contact:Email"],
                        Url = new Uri(configuration["SwaggerSettings:Contact:Url"])
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = configuration["SwaggerSettings:Security:Description"],
                    Name = configuration["SwaggerSettings:Security:Name"],
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}