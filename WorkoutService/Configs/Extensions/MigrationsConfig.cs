using WorkoutService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WorkoutService.Configs.Extensions
{
    public static class MigrationsConfig
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            return app;
        }

    }
}
