using Microsoft.EntityFrameworkCore;

namespace FCE.Domain
{
    public enum Status
    {
        Weak,
        Normal,
        Hard,
    }   
    public class CalculatedMetrics //stage 2 [Stores derived metabolic outputs]
    {
        public Guid userId { get; set; }
        public double BMR { get; set; }
        public double TDEE { get; set; }
        public double calorieTarget { get; set; }
        public Status status { get; set; }
        public DateTime CalculatedAt { get; set; }
    }

    public class CalculatedMetricsConfig //: IEntityTypeConfiguration<CalculatedMetrics>
    {
        
    }
}
