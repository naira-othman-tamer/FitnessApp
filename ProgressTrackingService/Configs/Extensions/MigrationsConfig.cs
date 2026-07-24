using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Infrastructure.Data.Seed;
using ProgressTrackingService.Infrastructure.Data;

namespace ProgressTrackingService.Configs.Extensions;

public static class MigrationsConfig
{
    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
            await context.Database.MigrateAsync();
        await DBSeeder.SeedAsync(context);
        return app;
    }

}
