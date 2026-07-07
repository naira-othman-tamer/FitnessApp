using MediatR;
using WorkoutService.Domain.Enums;
using WorkoutService.Domain.Entities;
using WorkoutService.Infrastructure;
using WorkoutService.Features.Common.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutService.Features.Workouts.CreateWorkout
{
    public record CreateWorkoutCommand
    (
     string Name,
     WorkoutCategory Category,
     int DurationInMinutes,
     string? ImageUrl,
     bool IsPremium = false
    ) : IRequest<RequestResult<bool>>;

    public class CreateWorkoutCommandHandler : IRequestHandler<CreateWorkoutCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<Domain.Entities.Workout> _workoutRepository;

        public CreateWorkoutCommandHandler(GeneralRepository<Domain.Entities.Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }

        public async Task<RequestResult<bool>> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
        {
            _workoutRepository.Add(new Domain.Entities.Workout
            {
                Name = request.Name,
                Category = request.Category,
                DurationInMinutes = request.DurationInMinutes,
                CaloriesBurn = 350,
                ImageUrl = request.ImageUrl,
                IsPremium = request.IsPremium
            });
            await _workoutRepository.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true);
        }
    }

    public static class CreateWorkoutEndPoint
    {
        public static void AddWorkoutEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
                [FromBody] CreateWorkoutCommand request,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return Results.Ok(result);
            });
        }
    }
}
