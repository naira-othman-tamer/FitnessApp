using ContractMessages.Enums;
using FCE.Domain.Aggregates;
using FCE.Domain.Enums;
using FCE.Domain.ValueObject;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCE.Features.Stats.SubmitFitnessStats
{
    public record SubmitStateCommand
    (Guid userId,
      PhysicalStats PhysicalStats,
      Goal goal,
      ActivityLevel activityLevel,
      bool IsActive) : ICommandRequest<RequestResult<int>>; 

   public class SubmitStateCommandHandler : IRequestHandler<SubmitStateCommand, RequestResult<int>>
    {
        private readonly GeneralRepository<UserFitnessStats> _userStatsRepo;
        public SubmitStateCommandHandler(GeneralRepository<UserFitnessStats> userStatsRepo)
        {
            _userStatsRepo = userStatsRepo;
        }
        public async Task<RequestResult<int>> Handle(SubmitStateCommand request, CancellationToken cancellationToken)
        {
            
            var userFitnessStats = new UserFitnessStats
            {
                userId = request.userId,
                PhysicalStats = request.PhysicalStats,
                goal = request.goal,
                activityLevel = request.activityLevel,
                IsActive = request.IsActive
            };
            _userStatsRepo.Add(userFitnessStats);
            await _userStatsRepo.SaveChangesAsync();
            return RequestResult<int>.Success(userFitnessStats.Id);
        }
    }

    public class SubmitStateCommandValidator : AbstractValidator<SubmitStateCommand>
    {
        public SubmitStateCommandValidator()
        {
            RuleFor(x => x.PhysicalStats.Weight)
                .InclusiveBetween(40, 200)
                .WithMessage("Weight must be between 40 and 200 kg");

            RuleFor(x => x.PhysicalStats.Height)
                .InclusiveBetween(140, 220)
                .WithMessage("Height must be between 140 and 220 cm");

            RuleFor(x => x.PhysicalStats.Age)
                .InclusiveBetween((short)16, (short)100)
                .WithMessage("Age must be between 16 and 100");

            RuleFor(x => x.PhysicalStats.Gender)
                .IsInEnum()
                .WithMessage("Gender must be Male or Female");

            RuleFor(x => x.goal)
                .IsInEnum()
                .WithMessage("Goal must be a valid fitness goal");

            RuleFor(x => x.activityLevel)
                .IsInEnum()
                .WithMessage("Activity level must be a valid activity level");

            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("UserId is required");
        }
    };

    public static class SubmitFitnessEndPoint
    {
        public static void SubmitFitnessStateEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
               [FromBody] SubmitStateCommand request,
               [FromServices] IMediator mediator
                ) =>
            {
                var id = await mediator.Send(request);
                return Results.Created($"stats/{id}", new { id });
            });
        }

    }
 }
