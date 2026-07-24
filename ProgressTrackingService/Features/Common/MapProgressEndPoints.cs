namespace ProgressTrackingService.Features.Common
{
    public static class MapProgressEndPoints
    {
        public static IEndpointRouteBuilder MapProgressServiceEndPoints(this IEndpointRouteBuilder builder)
        {
            var planGroup = builder.MapGroup("plan");
          

            return builder;
        }
    }
}
