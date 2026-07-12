using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using ProfileService.Data;
using ProfileService.Infrastructure;
using Repository.Layer;
using Repository.Layer.Interfaces;

namespace ProfileService.Extensions;

public static class ProfileInfrastructureExtensions
{
    public static IServiceCollection AddProfileInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration is missing.");
        jwt.Validate();

        services.Configure<JwtOptions>(configuration.GetRequiredSection(JwtOptions.SectionName));
        services.AddHttpContextAccessor();
        services.AddScoped<IClaimsManager, ClaimsManager>();
        services.AddScoped(typeof(IUnitOfWork<ProfileDbContext>), typeof(UnitOfWork<ProfileDbContext>));
        services.AddScoped<IProfileDataSeeder, ProfileDataSeeder>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"] ?? "localhost:6379";
            options.InstanceName = "FitnessApp:";
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
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
