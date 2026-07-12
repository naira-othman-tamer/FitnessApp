using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace ProgressTrackingService.Extensions
{
    public static class JwtAuthenticationConfig
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
            var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
            var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
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
}
