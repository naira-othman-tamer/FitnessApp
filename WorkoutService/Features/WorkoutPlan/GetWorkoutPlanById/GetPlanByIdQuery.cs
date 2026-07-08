using ContractMessages.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.WorkoutPlan.GetWorkoutPlanById
{
    public record GetPlanByIdQuery (int PlanId) : IRequest<RequestResult<WorkoutPlanDto>>;

    public class GetPlanByIdQueryValidator : AbstractValidator<GetPlanByIdQuery>
    {
        public GetPlanByIdQueryValidator()
        {
            RuleFor(x => x.PlanId).GreaterThan(0)
                .WithMessage("Plan ID must be a positive integer.");
        }
    }
    public record WorkoutPlanDto
    (
         string PlanName ,
         string? PlanDescription,
         string? ImageUrl ,
         bool IsPremium ,
         Goal PlanGoal ,
         int WorkoutDaysPerWeek ,
         ICollection<PlanDay>? PlanDays 
    );

    public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, RequestResult<WorkoutPlanDto>>
    {
        private readonly GeneralRepository<Domain.Entities.WorkoutPlan> _workoutPlanRepository;
        public GetPlanByIdQueryHandler(GeneralRepository<Domain.Entities.WorkoutPlan> workoutPlanRepository)
        {
            _workoutPlanRepository = workoutPlanRepository;
        }
        public async Task<RequestResult<WorkoutPlanDto>> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
        {
            var workoutPlanDto = await _workoutPlanRepository.Get(p=>p.Id == request.PlanId)
                .Select(p => new WorkoutPlanDto
                (
                    p.Name,
                    p.Description,
                    p.ImageUrl,
                    p.IsPremium,
                    p.Goal,
                    p.WorkoutDaysPerWeek,
                    p.PlanDays
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (workoutPlanDto is null)
            {
                return RequestResult<WorkoutPlanDto>.Failure("Workout plan not found.", RequestErrorCode.NotFound);
            }

                return RequestResult<WorkoutPlanDto>.Success(workoutPlanDto);
        }
    }

    public static class GetPlanByIdQueryEndPoint
    {
        public static void GetPlanByIdEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/{id}", async (
                int id,
                [FromServices] IMediator mediator
               ) =>
            {
                var Plan = await mediator.Send(new GetPlanByIdQuery(id));
                if (!Plan.IsSuccess)
                {
                    return Results.NotFound(Plan.Message);
                }
                return Results.Ok(Plan.Data);
            });
        }
    }

}
