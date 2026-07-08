using MediatR;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Excercise.CreateExercies
{
    public record CreateExerciseCommand
    (string ExName,
       Difficulty ExDifficulty,
       IEnumerable<MuscleGroup> TargetMuscles,
       IEnumerable<EquipmentNeeded> EquipmentNeeded,
       string? Description = null,
       string? ImageUrl = null,
       string? VideoUrl = null
    ) : IRequest<bool>;

    public class CreateExerciseCommandHandler : IRequestHandler<CreateExerciseCommand, bool>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;
        public CreateExerciseCommandHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<bool> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
        {
            _exerciseRepository.Add(new Exercise
            {
                Name = request.ExName,
                Difficulty = request.ExDifficulty,
                TargetMuscles = request.TargetMuscles.ToList(),
                EquipmentNeeded = request.EquipmentNeeded.ToList(),
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                VideoUrl = request.VideoUrl
            });

            await _exerciseRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}
