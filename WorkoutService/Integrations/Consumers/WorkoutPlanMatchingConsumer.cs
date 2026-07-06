using ContractMessages.WorkoutPlanMatching;
using MassTransit;
//using MassTransit.Mediator;
using MediatR;
using WorkoutService.Features.Plan;
using WorkoutService.Features.Plan.MatchUserWorkoutPlan;


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


            await context.RespondAsync<IGetWorkoutPlanResponse>(new
            {
                WorkoutPlanId = plan.Id,
                Name = plan.Name
            });
        }
    }
}
