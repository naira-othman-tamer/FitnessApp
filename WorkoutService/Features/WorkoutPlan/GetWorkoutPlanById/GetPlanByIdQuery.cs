using ContractMessages.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.WorkoutPlan.GetWorkoutPlanById
{
    public record GetPlanByIdQuery (int PlanId) : IRequest<RequestResult<WorkoutPlanDto>>;

    public record WorkoutPlanDto
    (
         string PlanName ,
         string? PlanDescription,
         string? ImageUrl ,
         bool IsPremium ,
         Goal PlanGoal ,
         int WorkoutDaysPerWeek ,
         Difficulty Difficulty,
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
                    p.Difficulty,
                    p.PlanDays
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (workoutPlanDto == null)
            {
                return RequestResult<WorkoutPlanDto>.Failure("Workout plan not found.");
            }

                return RequestResult<WorkoutPlanDto>.Success(workoutPlanDto);
        }
    }

}
