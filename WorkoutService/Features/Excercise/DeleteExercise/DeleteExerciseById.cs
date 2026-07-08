using MediatR;
using WorkoutService.Domain.Entities;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Excercise.DeleteExercise
{
    public record DeleteExerciseById (int ExerciseId) : IRequest<bool>;

    public class DeleteExerciseByIdHandler : IRequestHandler<DeleteExerciseById, bool>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;

        public DeleteExerciseByIdHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public async Task<bool> Handle(DeleteExerciseById request, CancellationToken cancellationToken)
        {

            await _exerciseRepository.SoftDeleteById(request.ExerciseId, cancellationToken);
            
            return true;
        }
    }

}
