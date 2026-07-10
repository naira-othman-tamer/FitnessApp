using ContractMessages.Enums;

namespace ContractMessages.NutritionPlanMatching
{
    public interface IGetNutritionPlanRequest
    {
        Goal goal { get; }
        double CalroieTarget { get; }
    }
}
