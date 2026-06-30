namespace WorkoutService.Features.Common.Helpers
{
    public class RequestResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public RequestErrorCode? requestErrorCode { get; set; }

        public static RequestResult<T> Success(T data, string message = "Success", RequestErrorCode? errorCode = RequestErrorCode.None)
        {
            return new RequestResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                requestErrorCode = errorCode
            };
        }

        public static RequestResult<T> Failure(string message = "Request Failed", RequestErrorCode? errorCode = null)
        {
            return new RequestResult<T>
            {
                IsSuccess = false,
                Data = default,
                Message = message,
                requestErrorCode = errorCode
            };
        }
    }
}

