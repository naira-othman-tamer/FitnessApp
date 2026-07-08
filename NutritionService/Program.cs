using Microsoft.EntityFrameworkCore;
using NutritionService.Data;
using NutritionService.Extensions;
using NutritionService.Features.Nutrition;
using NutritionService.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NutritionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NutritionDatabase")));
builder.Services.AddNutritionInfrastructure(builder.Configuration);
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.CustomSchemaIds(type => (type.FullName ?? type.Name).Replace('+', '.')));
builder.Services.AddHealthChecks();

var app = builder.Build();

await app.MigrateAndSeedNutritionDatabaseAsync();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapNutritionEndpoints();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
