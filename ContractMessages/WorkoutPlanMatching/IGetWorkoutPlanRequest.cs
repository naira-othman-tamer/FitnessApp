using ContractMessages.Enums;

namespace ContractMessages.WorkoutPlanMatching
{
    public interface IGetWorkoutPlanRequest
    {
        Goal Goal { get; }
        int WorkoutDaysPerWeek { get; }
    }
}
