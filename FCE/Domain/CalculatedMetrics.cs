using FCE.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FCE.Domain
{
   
    public class CalculatedMetrics //stage 2 [Stores derived metabolic outputs]
    {
        public Guid userId { get; set; }
        public decimal BMR { get; set; }
        private decimal _TDEE { get; set; }
        public decimal calorieTarget { get; set; }
        public EnergyLevel status { get; set; }
        public DateTime CalculatedAt { get; set; }
    

       public decimal GetTDEE()
        {
            return _TDEE;
        }
    }
}

public class CalculatedMetricsConfig //: IEntityTypeConfiguration<CalculatedMetrics>
    {
        
    }

