using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.PlanDay.AddPlanDay
{
    public record SetPLanDayCommand(int DayNumber, string? DayLabel, int WorkoutId) : IRequest<RequestResult<bool>>;

    public class SetPLanDayCommandValidator : AbstractValidator<SetPLanDayCommand>
    {
        public SetPLanDayCommandValidator()
        {
            RuleFor(x => x.DayNumber)
                .InclusiveBetween(1, 7)
                .WithMessage("DayNumber must be between 1 and 7.");
            RuleFor(x => x.WorkoutId)
                .GreaterThan(0)
                .WithMessage("WorkoutId must be a positive integer.");
            RuleFor(x => x.DayLabel)
                .MaximumLength(100)
                .WithMessage("DayLabel cannot exceed 100 characters.");
        }
    }

    public class SetPLanDayCommandHandler : IRequestHandler<SetPLanDayCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<Domain.Entities.PlanDay> _planDayRepository;

        public SetPLanDayCommandHandler(GeneralRepository<Domain.Entities.PlanDay> planDayRepository)
        {
            _planDayRepository = planDayRepository;
        }

        public async Task<RequestResult<bool>> Handle(SetPLanDayCommand request, CancellationToken cancellationToken)
        {
            _planDayRepository.Add(new Domain.Entities.PlanDay
            {
                DayNumber = request.DayNumber,
                Label = request.DayLabel,
                WorkoutId = request.WorkoutId,
                CreatedAt = DateTime.UtcNow,
            });

            await _planDayRepository.SaveChangesAsync(cancellationToken);


            return RequestResult<bool>.Success(true, "Plan day added successfully");
        }
    }

    public static class AddPlanDayEndpoint
    {
        public static void AddPlanDayEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
               [FromBody] SetPLanDayCommand request,
               [FromServices] IMediator mediator
                ) =>
            {
                var result = await mediator.Send(request);
                return Results.Created();
            });
        }
    }
 }



