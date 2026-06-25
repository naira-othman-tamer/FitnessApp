using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Domain.Entities;

namespace ProfileService.Infrastructure;

public interface IProfileDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class ProfileDataSeeder(ProfileDbContext dbContext, IConfiguration configuration) : IProfileDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var userIdText = configuration["Seed:UserProfile:UserId"];
        if (!Guid.TryParse(userIdText, out var userId))
            userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        if (await dbContext.UserProfiles.AnyAsync(x => x.UserId == userId, cancellationToken))
            return;

        dbContext.UserProfiles.Add(new UserProfile
        {
            UserId = userId,
            FirstName = configuration["Seed:UserProfile:FirstName"] ?? "Normal",
            LastName = configuration["Seed:UserProfile:LastName"] ?? "User",
            Email = configuration["Seed:UserProfile:Email"] ?? "user@fitnessapp.local",
            PhoneNumber = configuration["Seed:UserProfile:PhoneNumber"] ?? "+201000000000",
            IsPremiumCached = false,
            MemberSince = DateTime.UtcNow,
            Preferences = new UserPreferences(),
            NotificationSettings = new NotificationSettings(),
            PrivacySettings = new PrivacySettings()
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
