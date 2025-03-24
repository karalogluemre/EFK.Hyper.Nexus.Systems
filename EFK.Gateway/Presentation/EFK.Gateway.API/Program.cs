using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(opt =>
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
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
//builder.Configuration.AddJsonFile("Microservices/package.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddSwaggerGen();

#region Ocelot
builder.Services.AddOcelot();

#endregion
var app = builder.Build();
app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseForwardedHeaders();

app.UseAuthentication();
app.UseAuthorization();
app.UseOcelot().Wait();

app.MapControllers();

app.Run();
