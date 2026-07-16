using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan
{
    public record CheckUserActivePlanQuery(Guid userId) : IRequest<RequestResult<bool>>;

    public class CheckUserActivePlanQueryValidator : AbstractValidator<CheckUserActivePlanQuery>
    {
        public CheckUserActivePlanQueryValidator()
        {
            RuleFor(x => x.userId).NotEmpty().WithMessage("User ID cannot be empty.");
        }
    }

    public class CheckUserActivePlanQueryHandler : IRequestHandler<CheckUserActivePlanQuery, RequestResult<bool>>
    {
        private readonly GeneralRepository<UserAssignedPlan> _planRepository;
        public CheckUserActivePlanQueryHandler(GeneralRepository<UserAssignedPlan> planRepository)
        {
            _planRepository = planRepository;
        }
        public async Task<RequestResult<bool>> Handle(CheckUserActivePlanQuery request, CancellationToken cancellationToken)
        {
            var hasActivePlan = await _planRepository
                .Get(p=>p.userId == request.userId && p.IsActive)
                .AnyAsync(cancellationToken);

            if (!hasActivePlan)
            {
                return RequestResult<bool>.Failure("User does not have an active plan.", RequestErrorCode.NotFound);
            }

            return RequestResult<bool>.Success(hasActivePlan);
        }
    }

}
