using ContractMessages.UserMetrics;
using MassTransit;

namespace NutritionService.Infrastructure;

public interface IFceClient
{
    Task<double?> GetCalorieTargetAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed class FceClient(IRequestClient<IGetUserMetricsRequest> requestClient, ILogger<FceClient> logger) : IFceClient
{
    public async Task<double?> GetCalorieTargetAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await requestClient.GetResponse<IGetUserMetricsResponse>(new
            {
                UserId = userId
            }, cancellationToken);

            return response.Message.IsSuccess ? response.Message.CalorieTarget : null;
        }
        catch (Exception exception) when (exception is RequestTimeoutException or RabbitMqConnectionException or OperationCanceledException)
        {
            logger.LogWarning(exception, "Could not retrieve calorie target for user {UserId} from FCE over RabbitMQ.", userId);
            return null;
        }
    }
}
