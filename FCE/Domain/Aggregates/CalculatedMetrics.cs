using FCE.Domain.Entities;
using FCE.Domain.Enums;
using FCE.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCE.Domain.Aggregates
{
    public class CalculatedMetrics : BaseEntity
    {
        public Guid UserId { get; set; }
        //public double BMR { get; private set; }
        //public double TDEE { get; private set; }
        //public double CalorieTarget { get; private set; }
        public MetabolicCalculator Result { get; private set; } 
       // public CalorieTarget Status { get; private set; }

        private CalculatedMetrics() { }

        public static CalculatedMetrics Calculate(Guid userId,PhysicalStats physicalStats,ActivityLevel activityLevel,Goal goal)
        {
            var bmr = CalculateBmr(physicalStats);  //from value object
            var tdee = CalculateTdee(bmr,activityLevel);
            var calorieTarget = CalculateCalorieTarget(tdee, goal);
            var tier = ClassifyTier(calorieTarget);

            return new CalculatedMetrics
            {
                UserId = userId,
                Result = new MetabolicCalculator(
                    Math.Round(bmr, 2),
                    Math.Round(tdee, 2),
                    Math.Round(calorieTarget, 2),
                    tier
                )
            };
        }
        public static CalculatedMetrics Calculate(UserFitnessStats stats)
        {
            var bmr = CalculateBmr(stats.PhysicalStats);  //from value object
            var tdee = CalculateTdee(bmr, stats.activityLevel);
            var calorieTarget = CalculateCalorieTarget(tdee, stats.goal);
            var tier = ClassifyTier(calorieTarget);

            return new CalculatedMetrics
            {
                UserId = stats.userId,
                Result = new MetabolicCalculator(
                    Math.Round(bmr, 2),
                    Math.Round(tdee, 2),
                    Math.Round(calorieTarget, 2),
                    tier
                )
            };
        }

        private static double CalculateBmr(PhysicalStats stats) =>
            stats.Gender == Gender.Male
                ? (10 * stats.Weight) + (6.25 * stats.Height) - (5 * stats.Age) + 5
                : (10 * stats.Weight) + (6.25 * stats.Height) - (5 * stats.Age) - 161;


        private static double CalculateTdee(double bmr, ActivityLevel level) =>
            bmr * level switch
            {
                ActivityLevel.Rookie => 1.2,
                ActivityLevel.Beginner => 1.375,
                ActivityLevel.Intermediate => 1.55,
                ActivityLevel.Advance => 1.725,
                ActivityLevel.TrueBeast => 1.9,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static double CalculateCalorieTarget(double tdee, Goal goal) =>
            goal switch
            {
                Goal.LoseWeight => tdee - 500,
                Goal.GainWeight => tdee + 300,
                Goal.GainMoreFlexible => tdee + 150,
                Goal.GetFitter => tdee,
                Goal.LearnTheBasic => tdee,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static CalorieTarget ClassifyTier(double calorieTarget) =>
            calorieTarget switch
            {
                <= 1800 => CalorieTarget.Low,
                > 1800 and <= 2500 => CalorieTarget.Moderate,
                _ => CalorieTarget.High
            };
    }

    public class CalculatedMetricsConfiguration : IEntityTypeConfiguration<CalculatedMetrics>
    {
        public void Configure(EntityTypeBuilder<CalculatedMetrics> builder)
        {
            builder.ToTable("CalculatedMetrics");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId).IsUnique();

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.OwnsOne(x => x.Result, r =>
            {
                r.Property(x => x.BMR)
                 .HasColumnName("BMR")
                 .IsRequired();

                r.Property(x => x.TDEE)
                 .HasColumnName("TDEE")
                 .IsRequired();

                r.Property(x => x.CalorieTarget)
                 .HasColumnName("CalorieTarget")
                 .IsRequired();

                r.Property(x => x.Tier)
                 .HasColumnName("CalorieTier")
                 .HasConversion<string>()
                 .HasMaxLength(20)
                 .IsRequired();
            });
        }
    }
}
    


