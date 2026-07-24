using FluentValidation;
using MediatR;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Features.Common.Helpers;
using ProgressTrackingService.Infrastructure;
using static ProgressTrackingService.Features.Common.Helpers.PaginationHelper;

namespace ProgressTrackingService.Features.Measurments.GetMeasurementHistoryForCurrentUser;

public record GetUserMeasurementsHistoryQuery(Guid userId, int Page , int PageSize) : IRequest<RequestResult<PaginatedMeasurementsDto>>;

public class GetFilteredWorkoutsQueryValidator : AbstractValidator<GetUserMeasurementsHistoryQuery>
{
    public GetFilteredWorkoutsQueryValidator()
    {
        RuleFor(x => x.userId)
            .NotEmpty()
            .WithMessage("User id is required.");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.");

    }
}
public record PaginatedMeasurementsDto
(
     int page,
     int pageSize,
     int TotalCount,
     int TotalPages,
    IEnumerable<MeasurmentDto> MeasurmentsList
);

public record MeasurmentDto
(
    double WeightKg,
    double? BodyFatPercent,
    DateTime RecordedAt,
    string? Notes
);

public class GetUserMeasurementsHistoryQueryHandler : IRequestHandler<GetUserMeasurementsHistoryQuery, RequestResult<PaginatedMeasurementsDto>>
{
    private readonly GeneralRepository<BodyMeasurements> _measurementsRepo;

    public GetUserMeasurementsHistoryQueryHandler(GeneralRepository<BodyMeasurements> measurementsRepo)
    {
        _measurementsRepo = measurementsRepo;
    }

    public async Task<RequestResult<PaginatedMeasurementsDto>> Handle(GetUserMeasurementsHistoryQuery request, CancellationToken cancellationToken)
    {
       var pagedMeasurements = await _measurementsRepo.GetAll()
            .Where(m=>m.UserId==request.userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(m=> new MeasurmentDto
            (
                m.WeightKg,
                m.BodyFatPercent,
                m.CreatedAt,
                m.Notes
            )).ToPaginatedAsync(request.Page, request.PageSize, cancellationToken);

        if (pagedMeasurements.Data.Count() == 0)
        {
            return RequestResult<PaginatedMeasurementsDto>.Failure("No Measurments found for Current User", RequestErrorCode.NotFound);
        }


        var PaginatedResult = new PaginatedMeasurementsDto(
            request.Page,
            request.PageSize,
            pagedMeasurements.TotalCount,
            pagedMeasurements.TotalPages,
            pagedMeasurements.Data); 

        return RequestResult<PaginatedMeasurementsDto>.Success(PaginatedResult);
    }
}
