using ProgressTrackingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ProgressTrackingService.Configs.Extensions;

public static class ContextConfig
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("cs"))
               .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
               .LogTo(log => Debug.WriteLine(log), LogLevel.Information).
               EnableSensitiveDataLogging()

        );

        return services;
    }
}
