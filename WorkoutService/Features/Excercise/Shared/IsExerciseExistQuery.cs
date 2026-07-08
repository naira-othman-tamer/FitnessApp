using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Excercise.Shared
{
    public record IsExerciseExistQuery(int exerciseId) : IRequest<RequestResult<bool>>;

    public class IsExerciseExistQueryHandler : IRequestHandler<IsExerciseExistQuery, RequestResult<bool>>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;
        public IsExerciseExistQueryHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<RequestResult<bool>> Handle(IsExerciseExistQuery request, CancellationToken cancellationToken)
        {
            var exists = await _exerciseRepository
                .Get(e => e.Id == request.exerciseId)
                .AnyAsync(cancellationToken);
            if (!exists)
            {
                return RequestResult<bool>.Failure($"Exercise with ID {request.exerciseId} not found.");
            }
            return RequestResult<bool>.Success(true);
        }
    }

}
