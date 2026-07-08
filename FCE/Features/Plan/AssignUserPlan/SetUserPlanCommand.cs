using ContractMessages.Enums;
using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using FluentValidation;
using MediatR;

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
        ) : ICommandRequest<RequestResult<int>>;

    public class SetUserPlanCommandValidator : AbstractValidator<SetUserPlanCommand>
    {
        public SetUserPlanCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty().WithMessage("User ID cannot be empty.");
            RuleFor(x => x.userGoal).IsInEnum().WithMessage("Invalid user goal.");
            RuleFor(x => x.IntakeClaorie).GreaterThan(0).WithMessage("Calorie intake must be greater than zero.");
            RuleFor(x => x.WorkoutPlanName).NotEmpty().WithMessage("Workout plan name cannot be empty.");
            //RuleFor(x => x.NutritionPlanName).NotEmpty().WithMessage("Nutrition plan name cannot be empty.");
        }
    }
    public class AssignUserPlanCommandHandler : IRequestHandler<SetUserPlanCommand, RequestResult<int>>
    {
        private readonly GeneralRepository<UserAssignedPlan> _userPlanRepo;

        public AssignUserPlanCommandHandler(GeneralRepository<UserAssignedPlan> userPlanRepo)
        {
            _userPlanRepo = userPlanRepo;
        }

        public async Task<RequestResult<int>> Handle(SetUserPlanCommand request, CancellationToken cancellationToken)
        {
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
           await _userPlanRepo.SaveChangesAsync();

           return RequestResult<int>.Success(plan.Id);
        }
    }
}
