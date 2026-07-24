using MediatR;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Features.Common.Helpers;
using ProgressTrackingService.Infrastructure;

namespace ProgressTrackingService.Features.Measurments.AddBodyMeasurments
{
    public record SetBodyMeasurmentCommand(Guid userId, double BodyWieghtKg, double? BodyFatPercent, string? Notes) :
        ICommandRequest<RequestResult<bool>>;

    public class SetBodyMeasurmentCommandHandler : IRequestHandler<SetBodyMeasurmentCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<BodyMeasurements> _measurmentsRepo;
        public SetBodyMeasurmentCommandHandler(GeneralRepository<BodyMeasurements> measurmentsRepo)
        {
            _measurmentsRepo = measurmentsRepo;
        }

        public async Task<RequestResult<bool>> Handle(SetBodyMeasurmentCommand request, CancellationToken cancellationToken)
        {
            _measurmentsRepo.Add(new BodyMeasurements {
                UserId = request.userId,
                WeightKg = request.BodyWieghtKg,
                BodyFatPercent = request.BodyFatPercent,
                Notes = request.Notes });

            await _measurmentsRepo.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true);
        }
    }

}
