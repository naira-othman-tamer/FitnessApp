using MediatR;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Features.Common.Helpers;
using ProgressTrackingService.Infrastructure;
using System.Data.Entity;

namespace ProgressTrackingService.Features.Measurments.GetLatestMeasurement;

public record GetLatestUserMeasurementQuery(Guid UserId) : IRequest<RequestResult<usermeasurmentDto>>;

public record usermeasurmentDto
(
    double UserWeightKg,
    double? BodyFatPercent,
    DateTime recordedAt,
    string? Notes
);

public class GetLatestUserMeasurementQueryHandler : IRequestHandler<GetLatestUserMeasurementQuery, RequestResult<usermeasurmentDto>>
{
    private readonly GeneralRepository<BodyMeasurements> _measurmentsRepo;

    public GetLatestUserMeasurementQueryHandler(GeneralRepository<BodyMeasurements> measurmentsRepo)
    {
        _measurmentsRepo = measurmentsRepo;
    }

    public async Task<RequestResult<usermeasurmentDto>> Handle(GetLatestUserMeasurementQuery request, CancellationToken cancellationToken)
    {
        usermeasurmentDto? userMeasurments = await _measurmentsRepo.Get(m => m.UserId == request.UserId)
            .Select(m => new usermeasurmentDto
            (
                m.WeightKg,
                m.BodyFatPercent,
                m.CreatedAt,
                m.Notes
            )).FirstOrDefaultAsync(cancellationToken);

        if (userMeasurments is null)
        {
            return RequestResult<usermeasurmentDto>.Failure("Measurments NotFound",RequestErrorCode.NotFound);
        }

        return RequestResult<usermeasurmentDto>.Success(userMeasurments);
       
    }
}