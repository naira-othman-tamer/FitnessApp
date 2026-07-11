using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Extensions;
using ProfileService.Features.Profile;
using ProfileService.Features.Settings;
using ProfileService.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProfileDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProfileDatabase")));
builder.Services.AddProfileInfrastructure(builder.Configuration);
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => (type.FullName ?? type.Name).Replace('+', '.'));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
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
builder.Services.AddHealthChecks();

var app = builder.Build();

await app.MigrateAndSeedProfileDatabaseAsync();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapProfileEndpoints();
app.MapSettingsEndpoints();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
