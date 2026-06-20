namespace FCE.Domain
{
    public class FitnessPlanConfigs //stage 3 [Stores derived fitness plan outputs]
    {
        public string goal { get; set; }
        public string status { get; set; }
        public double calorieMin { get; set; }
        public double calorieMax { get; set; }
        public string externalPlanId { get; set; }
        public string planName { get; set; }
        public int workOutsPerWeek { get; set; }
    }
}
