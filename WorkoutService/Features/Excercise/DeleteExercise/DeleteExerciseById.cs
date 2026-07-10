using FluentValidation;
using MediatR;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Excercise.DeleteExercise
{
    public record DeleteExerciseById (int ExerciseId) : IRequest<RequestResult<bool>>;

    public class DeleteExerciseByIdValidator : AbstractValidator<DeleteExerciseById>
    {
        public DeleteExerciseByIdValidator()
        {
            RuleFor(x => x.ExerciseId)
                .GreaterThan(0)
                .WithMessage("Exercise ID must be a positive integer.");
        }
    }

    public class DeleteExerciseByIdHandler : IRequestHandler<DeleteExerciseById, RequestResult<bool>>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;

        public DeleteExerciseByIdHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public async Task<RequestResult<bool>> Handle(DeleteExerciseById request, CancellationToken cancellationToken)
        {

            await _exerciseRepository.SoftDeleteById(request.ExerciseId, cancellationToken);
            await _exerciseRepository.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }

}
