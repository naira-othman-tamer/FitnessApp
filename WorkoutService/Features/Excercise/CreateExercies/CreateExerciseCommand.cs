using FluentValidation;
using MediatR;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;
using WorkoutService.Features.Common.Helpers;
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
    ) : IRequest<RequestResult<bool>>;

    public class CreateExerciseCommandValidator : AbstractValidator<CreateExerciseCommand>
    {
        public CreateExerciseCommandValidator()
        {
            RuleFor(x => x.ExName)
                .NotEmpty().WithMessage("Exercise name is required.")
                .MaximumLength(100).WithMessage("Exercise name must not exceed 100 characters.");
            RuleFor(x => x.ExDifficulty)
                .IsInEnum().WithMessage("Invalid difficulty level.");
          RuleFor(x => x.EquipmentNeeded)
                .NotEmpty().WithMessage("At least one equipment needed is required.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
            RuleFor(x => x.ImageUrl)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .WithMessage("Image URL must be a valid absolute URL.");
            RuleFor(x => x.VideoUrl)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .When(x => !string.IsNullOrWhiteSpace(x.VideoUrl))
                .WithMessage("Video URL must be a valid absolute URL.");
        }
    }
    public class CreateExerciseCommandHandler : IRequestHandler<CreateExerciseCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;
        public CreateExerciseCommandHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<RequestResult<bool>> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
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
            return RequestResult<bool>.Success(true);
        }
    }

    public static class CreateExerciseEndpoint
    {
        public static void MapCreateExerciseEndpoint(this WebApplication app)
        {
            app.MapPost("/", async (CreateExerciseCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result);
            })
            .WithName("CreateExercise");
            //.WithTags("Exercises");
        }
    }

}
