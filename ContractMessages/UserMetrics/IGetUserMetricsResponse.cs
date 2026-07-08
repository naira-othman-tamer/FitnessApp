using ContractMessages.Enums;

namespace ContractMessages.UserMetrics;

public interface IGetUserMetricsResponse
{
    bool IsSuccess { get; }
    double CalorieTarget { get; }
    IntegrationErrorCode ErrorCode { get; }
}
