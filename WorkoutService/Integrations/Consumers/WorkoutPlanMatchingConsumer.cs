using ContractMessages.Enums;
using ContractMessages.WorkoutPlanMatching;
using MassTransit;
using MediatR;
using WorkoutService.Features.WorkoutPlan.MatchUserWorkoutPlan;


namespace WorkoutService.Integrations.Consumers
{
    public class WorkoutPlanMatchingConsumer : IConsumer<IGetWorkoutPlanRequest>
    {
        private readonly IMediator _mediator;

        public WorkoutPlanMatchingConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task Consume(ConsumeContext<IGetWorkoutPlanRequest> context)
        {
           var plan = await _mediator
               .Send(new MatchWorkoutPlanOrchestrator(
                   context.Message.Goal,
                   context.Message.WorkoutDaysPerWeek));

            if (!plan.IsSuccess || plan.Data is null)
            {
                await context.RespondAsync<IGetWorkoutPlanResponse>(new
                {
                    IsSuccess = false,
                    WorkoutPlanId = 0,
                    WorkoutPlanName = string.Empty,
                    ErrorCode = IntegrationErrorCode.NoMatchingPlanFound
                });
                return;
            }

            await context.RespondAsync<IGetWorkoutPlanResponse>(new
            {
                IsSuccess = true,
                WorkoutPlanId = plan.Data.Id,
                WorkoutPlanName = plan.Data.Name,
                ErrorCode = IntegrationErrorCode.None
            });
        }
    }
}
