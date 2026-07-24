using ContractMessages.Enums;
using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan.AssignUserPlan
{
    public record SetUserPlanCommand(
        Guid userId,
        Goal userGoal,
        double IntakeClaorie,
        string WorkoutPlanName,
        int? WorkoutPlanId,
        string NutritionPlanName,
        Guid? NutritionPlanId
        ) : ICommandRequest<RequestResult<SetUserPlanResult>>;

    public record SetUserPlanResult(int PlanId, bool ReplacedActivePlan);

    public class SetUserPlanCommandValidator : AbstractValidator<SetUserPlanCommand>
    {
        public SetUserPlanCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty().WithMessage("User ID cannot be empty.");
            RuleFor(x => x.userGoal).IsInEnum().WithMessage("Invalid user goal.");
            RuleFor(x => x.IntakeClaorie).GreaterThan(0).WithMessage("Calorie intake must be greater than zero.");
            RuleFor(x => x.WorkoutPlanName).NotEmpty().WithMessage("Workout plan name cannot be empty.");
            RuleFor(x => x.WorkoutPlanId).GreaterThan(0).When(x => x.WorkoutPlanId.HasValue)
                .WithMessage("Workout plan ID must be greater than zero.");
            RuleFor(x => x.NutritionPlanName).NotEmpty().WithMessage("Nutrition plan name cannot be empty.");
            RuleFor(x => x.NutritionPlanId).NotEmpty().WithMessage("Nutrition plan ID cannot be empty.");
        }
    }
    public class AssignUserPlanCommandHandler : IRequestHandler<SetUserPlanCommand, RequestResult<SetUserPlanResult>>
    {
        private readonly GeneralRepository<UserAssignedPlan> _userPlanRepo;

        public AssignUserPlanCommandHandler(GeneralRepository<UserAssignedPlan> userPlanRepo)
        {
            _userPlanRepo = userPlanRepo;
        }

        public async Task<RequestResult<SetUserPlanResult>> Handle(SetUserPlanCommand request, CancellationToken cancellationToken)
        {
            var activePlans = await _userPlanRepo
                .Get(x => x.userId == request.userId && x.IsActive)
                .ToListAsync(cancellationToken);
            var replacedActivePlan = activePlans.Count > 0;

            foreach (var activePlan in activePlans)
            {
                activePlan.Deactivate();
                _userPlanRepo.Update(activePlan);
            }

            var plan = UserAssignedPlan
                 .Create(
                 request.userId,
                 request.userGoal,
                 request.IntakeClaorie,
                 request.WorkoutPlanName,
                 request.WorkoutPlanId,
                 request.NutritionPlanName,
                 request.NutritionPlanId);

            _userPlanRepo.Add(plan);
            await _userPlanRepo.SaveChangesAsync(cancellationToken);

            return RequestResult<SetUserPlanResult>.Success(new SetUserPlanResult(plan.Id, replacedActivePlan));
        }
    }
}
