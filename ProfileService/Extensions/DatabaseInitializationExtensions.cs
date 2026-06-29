using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Infrastructure;

namespace ProfileService.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task MigrateAndSeedProfileDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("ProfileDatabaseInitialization");

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<ProfileDbContext>();
            await context.Database.MigrateAsync();
            await scope.ServiceProvider.GetRequiredService<IProfileDataSeeder>()
                .SeedAsync(CancellationToken.None);
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Profile database migration or seeding failed.");
            throw;
        }
    }
}
