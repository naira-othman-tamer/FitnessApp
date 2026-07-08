using ContractMessages.Enums;

namespace ContractMessages.WorkoutPlanMatching
{
    public interface IGetWorkoutPlanResponse
    {
        bool IsSuccess { get; }
        int WorkoutPlanId { get; }
        string WorkoutPlanName { get; }
        IntegrationErrorCode ErrorCode { get; }
    }
}
