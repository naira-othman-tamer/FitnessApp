namespace ProgressTrackingService.Features.Common.Helpers
{
    public enum RequestErrorCode
    {
        None,
        NotFound,
        ValidationError,
        Conflict,
        DuplicateEntry,
        DependencyFailure,
        DependencyInUse,
        WorkoutNotFound
    }
}
