using Microsoft.EntityFrameworkCore;
using WorkoutService.Infrasructure.Data.DataSeeder;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Configs.Extensions
{
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
}
