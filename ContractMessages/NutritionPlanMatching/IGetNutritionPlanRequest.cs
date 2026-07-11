using ContractMessages.Enums;

namespace ContractMessages.NutritionPlanMatching
{
    public interface IGetNutritionPlanRequest
    {
        Goal Goal { get; }
        double CalorieTarget { get; }
    }
}
