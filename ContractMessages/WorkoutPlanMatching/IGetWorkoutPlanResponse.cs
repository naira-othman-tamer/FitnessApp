namespace ContractMessages.WorkoutPlanMatching
{
    public interface IGetWorkoutPlanResponse
    {
        Guid WorkoutPlanId { get; }
        string Name { get; }
    }
}
