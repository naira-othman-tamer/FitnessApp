using Microsoft.EntityFrameworkCore;
using Workout.Infrastructure.Data.Seed;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Configs.Extensions
{
    public static class MigrationsConfig
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            await context.Database.MigrateAsync();
            await DBSeeder.SeedAsync(context);
            return app;
        }

    }
}
