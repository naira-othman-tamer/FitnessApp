using FCE.Domain.Enums;

namespace FCE.Domain
{
    public class PlanAssignmentRule //stage 3 [Stores derived fitness plan outputs]
    {
        public Goal goal { get; set; }
       public EnergyLevel status { get; set; }
        public decimal? calorieMin { get; set; }
        public decimal? calorieMax { get; set; }
        public string externalPlanId { get; set; }
        //public string PlanName { get; set; }
        public int WorkOutsPerWeek { get; set; }
    }
}
