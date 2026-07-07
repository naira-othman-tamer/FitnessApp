using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Domain.Enums;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Features.Workouts.GetWorkoutById;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Workouts.GetWorkoutsList
{
    public record GetAllWorkoutsQuery (int page, int pageSize) : IRequest<RequestResult<GetWorkoutsListDto>>;

    public record GetWorkoutsListDto
    (
        int pageNumber,
        int pageSize,
        int TotalCount,
        List<GetAllWorkoutsDto> Workouts
    );

    public record GetAllWorkoutsDto
    (
        int Id,
        string Name,
        WorkoutCategory WCategory,
        int DurationInMinutes ,
        string? ImageUrl,
        bool IsPremium 
    );

    public class GetAllWorkoutsQueryHandler : IRequestHandler<GetAllWorkoutsQuery, RequestResult<GetWorkoutsListDto>>
    {
        private readonly GeneralRepository<Domain.Entities.Workout> _workoutRepository;
        public GetAllWorkoutsQueryHandler(GeneralRepository<Domain.Entities.Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }
        public async Task<RequestResult<GetWorkoutsListDto>> Handle(GetAllWorkoutsQuery request, CancellationToken cancellationToken)
        {
            var (workouts, totalCount, totalPages) = await _workoutRepository
                .GetAll()
                .ToPaginatedAsync(request.page, request.pageSize, cancellationToken);

            var workoutsDto = workouts.Select(w => new GetAllWorkoutsDto
            (
                  Id: w.Id,
                  Name: w.Name,
                  WCategory : w.Category,
                  DurationInMinutes : w.DurationInMinutes,
                  ImageUrl : w.ImageUrl,
                  IsPremium : w.IsPremium
             ));

            var result = new GetWorkoutsListDto
             (
                pageNumber: request.page,
                pageSize: request.pageSize,
                TotalCount: totalCount,
                Workouts: workoutsDto.ToList()
            );


            return RequestResult<GetWorkoutsListDto>.Success(result);
       
        }
    }

    public static class GetWorkoutsEndPoint
    {
        public static void GetWorkoutsListEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/", async ([FromServices] IMediator mediator,
                [FromQuery] int page = 1, [FromQuery] int pageSize = 10)=>
            {
                var userResult = await mediator.Send(new GetAllWorkoutsQuery(page, pageSize));
                return Results.Ok(userResult.Data);
            });
        }
    }
}
