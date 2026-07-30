using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Features.Goal.EndCurrentGoal;
using ProgressTrackingService.Features.Goal.GetCurrentGoal;
using ProgressTrackingService.Features.Goal.UpdateUserGoalOrDateTarget;
using ProgressTrackingService.Features.Measurments.AddBodyMeasurments;
using ProgressTrackingService.Features.Measurments.GetLatestMeasurement;
using ProgressTrackingService.Features.Measurments.GetMeasurementHistoryForCurrentUser;

namespace ProgressTrackingService.Features.Common
{
    public static class MapProgressEndPoints
    {
        public static IEndpointRouteBuilder MapProgressServiceEndPoints(this IEndpointRouteBuilder builder)
        {
            var MeasurmentsGroup = builder.MapGroup("BodyMeasurment");

            // POST /BodyMeasurment - Add body measurements
            MeasurmentsGroup.MapPost("/", async (
                [FromBody] SetBodyMeasurmentCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Message);
            }).WithName("AddBodyMeasurement");

            // GET /BodyMeasurment/latest - Get latest measurement
            MeasurmentsGroup.MapGet("/latest", async (
                [FromQuery] Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetLatestUserMeasurementQuery(userId);
                var result = await mediator.Send(query, cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
            }).WithName("GetLatestMeasurement");

            // GET /BodyMeasurment/history - Get measurement history
            MeasurmentsGroup.MapGet("/history", async (
                [FromQuery] Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10) =>
            {
                var query = new GetUserMeasurementsHistoryQuery(userId, page, pageSize);
                var result = await mediator.Send(query, cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
            }).WithName("GetMeasurementHistory");

            var ProgressGoalGroup = builder.MapGroup("ProgressTarget");

            // GET /ProgressTarget/current - Get current goal
            ProgressGoalGroup.MapGet("/current", async (
                [FromQuery] Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetCurrentUserGoalQuery(userId);
                var result = await mediator.Send(query, cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
            }).WithName("GetCurrentGoal");

            // PUT /ProgressTarget - Update goal
            ProgressGoalGroup.MapPut("/", async (
                [FromBody] UpdateUserProgressCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Message);
            }).WithName("UpdateGoal");

            // POST /ProgressTarget/end - End current goal
            ProgressGoalGroup.MapPost("/end", async (
                [FromBody] EndCurrentUserGoalCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(command, cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Message);
            }).WithName("EndCurrentGoal");

            return builder;
        }
    }
}
