namespace NutritionService.Common
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string? ErrorMessage { get; set; }

        // New attributes for richer debugging
        public string? Path { get; set; }
        public string? QueryString { get; set; }
        public string? Endpoint { get; set; }
        public Dictionary<string, string?> RouteValues { get; set; } = new();
        public string? ExceptionType { get; set; }
        public string? Source { get; set; }
        public string? TargetSite { get; set; }
        public string? StackTrace { get; set; }
        public string? User { get; set; }
        public DateTimeOffset Utc { get; set; } = DateTimeOffset.UtcNow;
    }
}
