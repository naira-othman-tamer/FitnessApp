using Microsoft.EntityFrameworkCore;
using NutritionService.Data;
using NutritionService.Infrastructure;

namespace NutritionService.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task MigrateAndSeedNutritionDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<NutritionDbContext>();
            await context.Database.MigrateAsync();
            await scope.ServiceProvider.GetRequiredService<INutritionDataSeeder>().SeedAsync(CancellationToken.None);
        }
        catch (Exception exception)
        {
            scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("NutritionDatabaseInitialization")
                .LogCritical(exception, "Nutrition database migration or seeding failed.");
            throw;
        }
    }
}
