using ContractMessages.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.WorkoutPlan.CreateWorkoutPlan
{
    public record SetWorkoutPlanCommand
     (
          string Name,
          string? Description,
          string? ImageUrl,
          Goal Goal,
          int WorkoutDaysPerWeek,
          ICollection<PlanDay> PlanDays
     ) : IRequest<RequestResult<bool>>;

    public class SetWorkoutPlanCommandValidator : AbstractValidator<SetWorkoutPlanCommand>
    {
        public SetWorkoutPlanCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100)
                .WithMessage("Workout plan name is required and must be at most 100 characters long.");

            RuleFor(x => x.WorkoutDaysPerWeek).InclusiveBetween(1, 7)
                .WithMessage("Workout days per week must be between 1 and 7.");

            RuleFor(x => x.Goal).IsInEnum().WithMessage("Invalid goal specified.");
        }
    }

    public class SetWorkoutPlanCommandHandler : IRequestHandler<SetWorkoutPlanCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<Domain.Entities.WorkoutPlan> _workoutPlanRepository;
        public SetWorkoutPlanCommandHandler(GeneralRepository<Domain.Entities.WorkoutPlan> workoutPlanRepository)
        {
            _workoutPlanRepository = workoutPlanRepository;
        }
        public async Task<RequestResult<bool>> Handle(SetWorkoutPlanCommand request, CancellationToken cancellationToken)
        {
            var workoutPlan = new Domain.Entities.WorkoutPlan
            {
                Name = request.Name,
                Goal = request.Goal,
                WorkoutDaysPerWeek = request.WorkoutDaysPerWeek,
                IsPremium = false,
                Description = request.Description,
                ImageUrl = request.ImageUrl
            };
            foreach (var planDay in request.PlanDays)
            {
                workoutPlan.PlanDays.Add(planDay);
            }
            _workoutPlanRepository.Add(workoutPlan);
            await _workoutPlanRepository.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true);
        }
    }

    public static class CreateWorkoutPlanEndPoint
    {
        public static void AddPlanEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
               [FromBody] SetWorkoutPlanCommand request,
               [FromServices] IMediator mediator
                ) =>
            {
                var id = await mediator.Send(request);
                return Results.Created($"/{id}", new { id });
            });
        }
    }

}
