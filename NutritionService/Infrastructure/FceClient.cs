using System.Net.Http.Json;
using System.Text.Json;

namespace NutritionService.Infrastructure;

public interface IFceClient
{
    Task<double?> GetCalorieTargetAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed class FceClient(HttpClient httpClient, ILogger<FceClient> logger) : IFceClient
{
    public async Task<double?> GetCalorieTargetAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var path = $"metrics/{userId}?userId={userId}";
            using var response = await httpClient.GetAsync(path, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;

            var envelope = await response.Content.ReadFromJsonAsync<FceEnvelope>(cancellationToken: cancellationToken);
            return envelope?.IsSuccess == true ? envelope.Data?.CalorieTarget : null;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            logger.LogWarning(exception, "Could not retrieve calorie target for user {UserId} from FCE.", userId);
            return null;
        }
    }

    private sealed record FceEnvelope(bool IsSuccess, FceMetrics? Data);
    private sealed record FceMetrics(double CalorieTarget);
}
