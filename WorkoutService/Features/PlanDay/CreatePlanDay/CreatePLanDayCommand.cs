using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.PlanDay.CreatePlanDay
{
    public record CreatePLanDayCommand(int DayNumber, string? DayLabel, int WorkoutId) : ICommand<RequestResult<bool>>;

    public class CreatePLanDayCommandValidator : AbstractValidator<CreatePLanDayCommand>
    {
        public CreatePLanDayCommandValidator()
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

    public class CreatePLanDayCommandHandler : IRequestHandler<CreatePLanDayCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<Domain.Entities.PlanDay> _planDayRepository;

        public CreatePLanDayCommandHandler(GeneralRepository<Domain.Entities.PlanDay> planDayRepository)
        {
            _planDayRepository = planDayRepository;
        }

        public async Task<RequestResult<bool>> Handle(CreatePLanDayCommand request, CancellationToken cancellationToken)
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

    public static class CreatePlanDayEndpoint
    {
        public static void MapCreatePlanDayEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
               [FromBody] CreatePLanDayCommand request,
               [FromServices] IMediator mediator
                ) =>
            {
                var result = await mediator.Send(request);
                return Results.Created();
            });
        }
    }
 }



