using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Enums;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Workouts.GetWorkoutById
{
    public record GetWorkoutByIdQuery(int WorkoutId) : IRequest<RequestResult<WorkoutDto>>;

    public record WorkoutDto
    (
        string Name,
        WorkoutCategory Category,
        int DurationInMinutes,
        int CaloriesBurn,
        string? ImageUrl,
        bool IsPremium
    );

    public class GetWorkoutByIdQueryValidator : AbstractValidator<GetWorkoutByIdQuery>
    {
        public GetWorkoutByIdQueryValidator()
        {
            RuleFor(x => x.WorkoutId)
                .GreaterThan(0).WithMessage("Workout ID must be greater than 0.");
        }
    }
    public class GetWorkoutByIdQueryHandler : IRequestHandler<GetWorkoutByIdQuery, RequestResult<WorkoutDto>>
    {
        private readonly GeneralRepository<Domain.Entities.Workout> _workoutRepository;
        public GetWorkoutByIdQueryHandler(GeneralRepository<Domain.Entities.Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }
        public async Task<RequestResult<WorkoutDto>> Handle(GetWorkoutByIdQuery request, CancellationToken cancellationToken)
        {
            var workoutDto = await _workoutRepository.Get(w=>w.Id == request.WorkoutId)
                .Select(w => new WorkoutDto(
                    w.Name,
                    w.Category,
                    w.DurationInMinutes,
                    w.CaloriesBurn,
                    w.ImageUrl,
                    w.IsPremium
                )).FirstOrDefaultAsync(cancellationToken);

            if (workoutDto is null)
            {
                return RequestResult<WorkoutDto>.Failure("Workout not found.",RequestErrorCode.NotFound);
            }

            return RequestResult<WorkoutDto>.Success(workoutDto);
        }
    }

    public static class GetWorkoutByIdEndPoint
    {
        public static void GetWorkoutByIDEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/{workoutId}", async ([FromQuery] int WorkoutId,
               [FromServices] IMediator mediator) =>
            {
                var userResult = await mediator.Send(new GetWorkoutByIdQuery
                    (WorkoutId));
                return Results.Ok(userResult.Data);
            });
        }
    }

}
