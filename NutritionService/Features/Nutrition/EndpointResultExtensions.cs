namespace NutritionService.Features.Nutrition;

internal static class EndpointResultExtensions
{
    public static IResult ToHttpResult(this OperationResult result) => Results.Json(result, statusCode: (int)result.StatusCode);
}
