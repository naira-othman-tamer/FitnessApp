using System.Text;
using AuthenticationService.Data;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Repository.Layer;
using Repository.Layer.Interfaces;

namespace AuthenticationService.Extensions;

public static class AuthenticationInfrastructureExtensions
{
    public static IServiceCollection AddAuthenticationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration is missing.");
        jwt.Validate();

        services.Configure<JwtOptions>(configuration.GetRequiredSection(JwtOptions.SectionName));
        services.AddHttpContextAccessor();
        services.AddScoped<IClaimsManager, ClaimsManager>();
        services.AddScoped(typeof(IUnitOfWork<AuthenticationDbContext>), typeof(UnitOfWork<AuthenticationDbContext>));
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IOtpNotificationService, LoggingOtpNotificationService>();
        services.AddScoped<IProfileLifecyclePublisher, LoggingProfileLifecyclePublisher>();
        services.AddDistributedMemoryCache();
        services.AddScoped<IAccountStateCache, AccountStateCache>();
        services.AddScoped<IAccessTokenRevocationStore, AccessTokenRevocationStore>();
        services.AddScoped<IAuthenticationDataSeeder, AuthenticationDataSeeder>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = TokenService.CreateValidationParameters(jwt);
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var jti = context.Principal?.FindFirst("jti")?.Value;
                        if (jti is not null && await context.HttpContext.RequestServices
                                .GetRequiredService<IAccessTokenRevocationStore>()
                                .IsRevokedAsync(jti, context.HttpContext.RequestAborted))
                        {
                            context.Fail("Token has been revoked.");
                        }
                    }
                };
            });
        services.AddAuthorization();
        return services;
    }
}
