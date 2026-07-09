namespace FCE.Features.Common.Helpers
{
    public enum RequestErrorCode
    {
        None,
        NotFound,
        ValidationError,
        DuplicateEntry,
        Conflict,
        DependencyFailure,
        CalculationFailed,
        InvalidMetricsInput,
        UserStatsNotFound,
        WeightUpdateFailed,
        GetUserMetricsFailed,
        BioMetricsResetFailed
    }
}
