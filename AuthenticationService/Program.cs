using AuthenticationService.Data;
using AuthenticationService.Extensions;
using AuthenticationService.Features.Auth;
using AuthenticationService.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuthenticationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthenticationDatabase")));
builder.Services.AddAuthenticationInfrastructure(builder.Configuration);
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.CustomSchemaIds(type => (type.FullName ?? type.Name).Replace('+', '.')));
builder.Services.AddHealthChecks();

var app = builder.Build();

await app.MigrateAndSeedAuthenticationDatabaseAsync();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticationEndpoints();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
