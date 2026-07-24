namespace ProgressTrackingService.Features.Common
{
    public static class MapProgressEndPoints
    {
        public static IEndpointRouteBuilder MapProgressServiceEndPoints(this IEndpointRouteBuilder builder)
        {
            var MeasurmentsGroup = builder.MapGroup("BodyMeasurment");

            var ProgressGoalGroup = builder.MapGroup("ProgressTarget");
          

            return builder;
        }
    }
}
