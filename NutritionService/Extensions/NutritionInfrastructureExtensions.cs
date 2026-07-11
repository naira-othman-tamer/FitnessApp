using System.Text;
using ContractMessages.UserMetrics;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NutritionService.Data;
using NutritionService.Infrastructure;
using NutritionService.Integrations.Consumers;
using Repository.Layer;
using Repository.Layer.Interfaces;

namespace NutritionService.Extensions;

public static class NutritionInfrastructureExtensions
{
    public static IServiceCollection AddNutritionInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration is missing.");
        jwt.Validate();

        services.AddHttpContextAccessor();
        services.AddScoped<IClaimsManager, ClaimsManager>();
        services.AddScoped(typeof(IUnitOfWork<NutritionDbContext>), typeof(UnitOfWork<NutritionDbContext>));
        services.AddScoped<INutritionDataSeeder, NutritionDataSeeder>();
        services.AddScoped<IFceClient, FceClient>();

        services.AddMassTransit(x =>
        {
            x.AddRequestClient<IGetUserMetricsRequest>(new Uri("queue:fce-user-metrics"));
            x.AddConsumer<NutritionPlanMatchingConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "guest");
                    h.Password(configuration["RabbitMq:Password"] ?? "guest");
                });

                cfg.ReceiveEndpoint("nutrition-plan-matching", e =>
                {
                    e.ConfigureConsumer<NutritionPlanMatchingConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        services.AddAuthorization();
        return services;
    }
}
