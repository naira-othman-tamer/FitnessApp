using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ContractMessages.UserMetrics;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
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
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"] ?? "localhost:6379";
            options.InstanceName = "FitnessApp:";
        });

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
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                              ?? context.Principal?.FindFirst("jti")?.Value;

                    if (string.IsNullOrWhiteSpace(jti))
                    {
                        context.Fail("Token does not include a JWT ID.");
                        return;
                    }

                    try
                    {
                        var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
                        if (await cache.GetStringAsync($"auth:revoked:{jti}", context.HttpContext.RequestAborted) is not null)
                        {
                            context.Fail("Token has been revoked.");
                        }
                    }
                    catch
                    {
                        context.Fail("Token revocation check is unavailable.");
                    }
                }
            };
        });
        services.AddAuthorization();
        return services;
    }
}
