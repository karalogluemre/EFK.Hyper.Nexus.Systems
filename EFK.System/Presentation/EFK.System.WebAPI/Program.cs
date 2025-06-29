using Commons.Persistence.Injection;
using Commons.Persistence.Registrations;
using Insure.Persistence.Context;
using Insure.Persistence.Injection;
using WatchDog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddPersistenceServices<ApplicationDbContext>(builder.Configuration, "Admin", "AdminLog");
builder.Services.AddIdentityRegistration<ApplicationDbContext>(builder.Configuration);
builder.Services.FeaturesDependencyInjectionServices(builder.Configuration);

var app = builder.Build();
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        options.RoutePrefix = string.Empty;
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseWatchDogExceptionLogger();

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();
app.UseWatchDog(opt =>
{
    opt.WatchPageUsername = "admin";
    opt.WatchPagePassword = "1Admin++";

});

app.MapControllers();
app.Run();
