using ContractMessages.Enums;
using ContractMessages.UserMetrics;
using FCE.Features.Metrics.GetUserCurrentMetrics;
using MassTransit;
using MediatR;

namespace FCE.Integrations.Consumers;

public sealed class UserMetricsConsumer(IMediator mediator) : IConsumer<IGetUserMetricsRequest>
{
    public async Task Consume(ConsumeContext<IGetUserMetricsRequest> context)
    {
        var result = await mediator.Send(new GetUserMetricsQuery(context.Message.UserId), context.CancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            await context.RespondAsync<IGetUserMetricsResponse>(new
            {
                IsSuccess = false,
                CalorieTarget = 0d,
                ErrorCode = IntegrationErrorCode.UserMetricsNotFound
            });
            return;
        }

        await context.RespondAsync<IGetUserMetricsResponse>(new
        {
            IsSuccess = true,
            result.Data.CalorieTarget,
            ErrorCode = IntegrationErrorCode.None
        });
    }
}
