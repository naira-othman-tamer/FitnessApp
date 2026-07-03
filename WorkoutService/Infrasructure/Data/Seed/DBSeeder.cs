using ContractMessages.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;
using WorkoutService.Domain.ValueObject;
using WorkoutService.Infrastructure.Data;

namespace Workout.Infrastructure.Data.Seed
{
    public static class DBSeeder
    {
        public static async Task SeedAsync(Context context)
        {
            if (await context.Exercises.AnyAsync())
                return; 

        

            await context.SaveChangesAsync();
        }

       
    }
}