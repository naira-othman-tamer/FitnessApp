using ContractMessages.Enums;
namespace ContractMessages.NutritionPlanMatching
{
    public interface IGetNutritionPlanResponse
    {
        Guid NutritionPlanId { get; }
        string NutritionPlanName { get; }
        IntegrationErrorCode ErrorCode { get; }
        bool IsSuccess { get; }
    }
}
