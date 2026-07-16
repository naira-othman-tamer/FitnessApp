using ContractMessages.Enums;
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
        public double BMR { get; private set; }
        public double TDEE { get; private set; }
        public double CalorieTarget { get; private set; } // => sent to Nutrition Service to select Nutrition Plan
        public BMRStatus BMRStatus { get; private set; }
        public BMRRange BMRRange { get; private set; }
        private CalculatedMetrics() { }

        public static CalculatedMetrics Calculate(UserFitnessStats stats)
        {
            var bmr = CalculateBmr(stats.PhysicalStats); 
            var tdee = CalculateTdee(bmr, stats.activityLevel);
            var calorieTarget = CalculateCalorieTarget(tdee, stats.goal);
            var BMRRange = GetBMRRange(stats.PhysicalStats.Gender);

            return new CalculatedMetrics
            {
                UserId = stats.userId,
                BMR = Math.Round(bmr, 2),
                TDEE = Math.Round(tdee, 2),
                CalorieTarget = calorieTarget,
                BMRRange = GetBMRRange(stats.PhysicalStats.Gender),
                BMRStatus = GetBMRStatus(bmr, BMRRange)
            };
        }
        public static CalculatedMetrics Calculate(Guid userId,PhysicalStats stats,ActivityLevel activelvl,Goal goal,Gender gender)
        {
            var bmr = CalculateBmr(stats); 
            var tdee = CalculateTdee(bmr, activelvl);
            var calorieTarget = CalculateCalorieTarget(tdee,goal);
            var BMRRange = GetBMRRange(gender);

            return new CalculatedMetrics
            {
                UserId = userId,
                BMR = Math.Round(bmr, 2),
                TDEE = Math.Round(tdee, 2),
                CalorieTarget = calorieTarget,
                BMRRange = GetBMRRange(gender),
                BMRStatus = GetBMRStatus(bmr, BMRRange)
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
        private static BMRRange GetBMRRange(Gender gender) =>
       gender switch
       {
           Gender.Male => new MaleBMRRange(),
           Gender.Female => new FemaleBMRRange(),
           _ => throw new ArgumentOutOfRangeException(nameof(gender))

       };
        private static BMRStatus GetBMRStatus(double bmr,BMRRange range)
        {
            if (bmr >= range.Min && bmr <= range.Max)
                return BMRStatus.InRange;

            return BMRStatus.OutOfRange;
        }
    }

    public class CalculatedMetricsConfiguration : IEntityTypeConfiguration<CalculatedMetrics>
    {
        public void Configure(EntityTypeBuilder<CalculatedMetrics> builder)
        {
            builder.ToTable("CalculatedMetrics");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId).IsUnique(); // upsert key — one current snapshot per user

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.BMR)
                   .HasColumnName("BMR")
                   .IsRequired();

            builder.Property(x => x.TDEE)
                   .HasColumnName("TDEE")
                   .IsRequired();

            builder.Property(x => x.CalorieTarget)
                   .HasColumnName("CalorieTarget")
                   .IsRequired(); // was missing before

            builder.Property(x => x.BMRStatus)
                   .HasColumnName("BMRStatus")
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.OwnsOne(x => x.BMRRange, r =>
            {
                r.Property(x => x.Min)
                 .HasColumnName("BMRRangeMin")
                 .IsRequired();

                r.Property(x => x.Max)
                 .HasColumnName("BMRRangeMax")
                 .IsRequired();
            });
        }
    }
}
    


