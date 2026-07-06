using MediatR;
using WorkoutService.Domain.Entities;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Excercise.UpdateExercise
{
    public record UpdateExercise (int ExerciseId, string Name, string Description) : IRequest<bool>;

    public class UpdateExerciseHandler : IRequestHandler<UpdateExercise, bool>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;
        public UpdateExerciseHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<bool> Handle(UpdateExercise request, CancellationToken cancellationToken)
        {
            return true;
        }
    }

}
