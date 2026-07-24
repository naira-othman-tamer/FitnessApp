using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ContractMessages.Notifications;
using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan.CompleteUserPlan
{
    public record CompleteUserPlanCommand(Guid UserId, string? Email = null)
        : IRequest<RequestResult<bool>>;

    public class CompleteUserPlanCommandValidator : AbstractValidator<CompleteUserPlanCommand>
    {
        public CompleteUserPlanCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID cannot be empty.");
        }
    }

    public class CompleteUserPlanCommandHandler : IRequestHandler<CompleteUserPlanCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<UserAssignedPlan> _assignedPlanRepo;
        private readonly GeneralRepository<UserPlanHistory> _planHistoryRepo;
        private readonly IPublishEndpoint _publishEndpoint;

        public CompleteUserPlanCommandHandler(
            GeneralRepository<UserAssignedPlan> assignedPlanRepo,
            GeneralRepository<UserPlanHistory> planHistoryRepo,
            IPublishEndpoint publishEndpoint)
        {
            _assignedPlanRepo = assignedPlanRepo;
            _planHistoryRepo = planHistoryRepo;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<RequestResult<bool>> Handle(CompleteUserPlanCommand request, CancellationToken cancellationToken)
        {
            var activePlan = await _assignedPlanRepo
                .Get(x => x.userId == request.UserId && x.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (activePlan is null)
            {
                return RequestResult<bool>.Failure("No active plan found for this user.", RequestErrorCode.NotFound);
            }

            activePlan.Deactivate();
            _assignedPlanRepo.Update(activePlan);

            _planHistoryRepo.Add(new UserPlanHistory
            {
                UserId = request.UserId,
                PlanId = activePlan.WorkoutPlanId ?? activePlan.Id,
                EndedAt = DateTime.UtcNow,
                ResonForChange = "Completed"
            });

            await _assignedPlanRepo.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                await _publishEndpoint.Publish<IEmailNotificationRequested>(new
                {
                    NotificationId = Guid.NewGuid(),
                    To = request.Email,
                    Subject = "Plan completed",
                    Body = $"""
                            <p>Great work completing your fitness plan.</p>
                            <p><strong>Workout plan:</strong> {activePlan.WorkoutPlan ?? "Completed plan"}</p>
                            <p><strong>Nutrition plan:</strong> {activePlan.NutritionPlan ?? "Completed nutrition plan"}</p>
                            <p>You can now request a new plan based on your latest progress.</p>
                            """,
                    IsHtml = true,
                    RequestedAtUtc = DateTime.UtcNow
                }, cancellationToken);
            }

            return RequestResult<bool>.Success(true, "Plan completed successfully.");
        }
    }

    public static class CompleteUserPlanEndpoint
    {
        public static void MapCompleteUserPlanEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("/{userId:guid}/complete", async (
                Guid userId,
                ClaimsPrincipal principal,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var email = principal.FindFirstValue(ClaimTypes.Email)
                    ?? principal.FindFirstValue(JwtRegisteredClaimNames.Email);

                var result = await mediator.Send(new CompleteUserPlanCommand(userId, email), cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Data);
                }

                return result.requestErrorCode == RequestErrorCode.NotFound
                    ? Results.NotFound(result.Message)
                    : Results.BadRequest(result.Message);
            });
        }
    }
}
