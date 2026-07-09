using FCE.Domain.Aggregates;
using FCE.Domain.Enums;
using FCE.Domain.ValueObject;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Metrics.GetUserCurrentMetrics
{
    public record GetUserMetricsQuery(Guid userId) : IRequest<RequestResult<userMetricsDTO>>;

  public class GetUserMetricsQueryValidator : AbstractValidator<GetUserMetricsQuery>
    {
        public GetUserMetricsQueryValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("UserId is required");
        }
    }

    public record userMetricsDTO
    (
        double userBMR,
        double userTDEE,
        BMRRange range,
        BMRStatus userTarget,
        double CalorieTarget 
    );

    public class GetUserMetricsQueryHandler : IRequestHandler<GetUserMetricsQuery, RequestResult<userMetricsDTO>> 
    {
        private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;

        public GetUserMetricsQueryHandler(GeneralRepository<CalculatedMetrics> metricsRepo)
        {
            _metricsRepo = metricsRepo;
        }

        public async Task<RequestResult<userMetricsDTO>> Handle(GetUserMetricsQuery request, CancellationToken cancellationToken)
        {
            var usermetrics = await _metricsRepo.Get(u => u.UserId == request.userId)
                .Select(m => new userMetricsDTO
                (
                    m.BMR,
                    m.TDEE,
                    m.BMRRange,
                    m.BMRStatus,
                    m.CalorieTarget
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (usermetrics is null)
            {
                return RequestResult<userMetricsDTO>.Failure("User metrics not found.", RequestErrorCode.NotFound);
            }

            return RequestResult<userMetricsDTO>.Success(usermetrics);
        }
    }

    public static class GetMetricsEndPoint
    {
        public static void GetUserMetricsEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/{userId}", async ([FromQuery] Guid userId,
               [FromServices] IMediator mediator) =>
            {
                var userResult = await mediator.Send(new GetUserMetricsQuery(userId));
                if (!userResult.IsSuccess)
                {
                    return Results.Problem(userResult.Message, statusCode: userResult.requestErrorCode == RequestErrorCode.NotFound ? 404 : 400);
                }
                return Results.Ok(userResult.Data);
            });
        }
    }

}
