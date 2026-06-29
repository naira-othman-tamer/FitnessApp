namespace ProfileService.Common.StandardizedResponse
{
    public static class OperationResultFactory
    {
        // success response
        public static OperationResult Success(
            string message = APIConstants.APIMessages.Success,
            string messageLocalized = APIConstants.APIMessages.SuccessLocalized,
            StatusCode statusCode = StatusCode.Success)
        {
            return new OperationResult(statusCode, message, messageLocalized);
        }

        // success response
        public static OperationResult<T> Success<T>(
            T data = default!,
            string message = APIConstants.APIMessages.Success,
            string messageLocalized = APIConstants.APIMessages.SuccessLocalized,
            StatusCode statusCode = StatusCode.Success)
        {
            return new OperationResult<T>(statusCode, message, messageLocalized, data);
        }

        // NoContent response
        public static OperationResult NoContent(
            string message = APIConstants.APIMessages.NoContent,
            string messageLocalized = APIConstants.APIMessages.NoContentLocalized,
            StatusCode statusCode = StatusCode.NoContent)
        {
            return new OperationResult(statusCode, message, messageLocalized);
        }

        // NoContent response
        public static OperationResult<T> NoContent<T>(
            T data = default!,
            string message = APIConstants.APIMessages.NoContent,
            string messageLocalized = APIConstants.APIMessages.NoContentLocalized,
            StatusCode statusCode = StatusCode.NoContent)
        {
            return new OperationResult<T>(statusCode, message, messageLocalized, data);
        }

        // error response
        public static OperationResult Error(
            string message = APIConstants.APIMessages.Error,
            string messageLocalized = APIConstants.APIMessages.ErrorLocalized,
            StatusCode statusCode = StatusCode.InternalServerError)
        {
            return new OperationResult(statusCode, message, messageLocalized);
        }

        // error response
        public static OperationResult<T> Error<T>(
            T data,
            string message = APIConstants.APIMessages.Error,
            string messageLocalized = APIConstants.APIMessages.ErrorLocalized,
            StatusCode statusCode = StatusCode.InternalServerError)
        {
            return new OperationResult<T>(statusCode, message, messageLocalized, data);
        }

        // error response
        public static OperationResult ServiceEligibleError(
            string message = APIConstants.APIMessages.Forbidden,
            string messageLocalized = APIConstants.APIMessages.ForbiddenLocalized,
            StatusCode statusCode = StatusCode.Forbidden)
        {
            return new OperationResult(statusCode, message, messageLocalized);
        }

        // error response
        public static OperationResult<T> ServiceEligibleError<T>(
            T data,
            string message = APIConstants.APIMessages.Forbidden,
            string messageLocalized = APIConstants.APIMessages.ForbiddenLocalized,
            StatusCode statusCode = StatusCode.Forbidden)
        {
            return new OperationResult<T>(statusCode, message, messageLocalized, data);
        }

        // bad request response
        public static OperationResult BadRequest(
            string message = APIConstants.APIMessages.BadRequest,
            string messageLocalized = APIConstants.APIMessages.BadRequestLocalized)
        {
            return new OperationResult(StatusCode.BadRequest, message, messageLocalized);
        }

        // bad request response
        public static OperationResult<T> BadRequest<T>(
            string message = APIConstants.APIMessages.BadRequest,
            string messageLocalized = APIConstants.APIMessages.BadRequestLocalized)
        {
            return new OperationResult<T>(StatusCode.BadRequest, message, messageLocalized);
        }

        // not found response
        public static OperationResult NotFound(
            string message = APIConstants.APIMessages.NotFound,
            string messageLocalized = APIConstants.APIMessages.NotFoundLocalized)
        {
            return new OperationResult(StatusCode.NotFound, message, messageLocalized);
        }

        // not found response
        public static OperationResult<T> NotFound<T>(
            string message = APIConstants.APIMessages.NotFound,
            string messageLocalized = APIConstants.APIMessages.NotFoundLocalized)
        {
            return new OperationResult<T>(StatusCode.NotFound, message, messageLocalized);
        }


        // unauthorized response 

        public static OperationResult UnAuthorized(
            string message = APIConstants.UserMessages.InvalidPassword,
            string messageLocalized = APIConstants.UserMessages.InvalidPasswordLocalized)
        {
            return new OperationResult(StatusCode.Unauthorized, message, messageLocalized);
        }

        // unauthorized response 

        public static OperationResult<T> UnAuthorized<T>(
            string message = APIConstants.UserMessages.InvalidPassword,
            string messageLocalized = APIConstants.UserMessages.InvalidPasswordLocalized)
        {
            return new OperationResult<T>(StatusCode.Unauthorized, message, messageLocalized);
        }


        // not verified response

        public static OperationResult NotVerified(
            string message = APIConstants.UserMessages.NotVerified,
            string messageLocalized = APIConstants.UserMessages.NotFoundLocalized)
        {
            return new OperationResult(StatusCode.Forbidden, message, messageLocalized);
        }



        // not verified response

        public static OperationResult<T> NotVerified<T>(
            string message = APIConstants.UserMessages.NotVerified,
            string messageLocalized = APIConstants.UserMessages.NotFoundLocalized)
        {
            return new OperationResult<T>(StatusCode.Forbidden, message, messageLocalized);
        }



        // Suspended response 

        public static OperationResult Suspended(
            string message = APIConstants.UserMessages.Suspended,
            string messageLocalized = APIConstants.UserMessages.SuspendedLocalized)
        {
            return new OperationResult(StatusCode.TooManyRequests, message, messageLocalized);
        }


        // Suspended response 

        public static OperationResult<T> Suspended<T>(
            string message = APIConstants.UserMessages.Suspended,
            string messageLocalized = APIConstants.UserMessages.SuspendedLocalized)
        {
            return new OperationResult<T>(StatusCode.TooManyRequests, message, messageLocalized);
        }

        // DataCorruption response
        public static OperationResult DataCorruption(
            string message,
            string messageLocalized = APIConstants.APIMessages.DataCorruptionLocalized)
        {
            return new OperationResult(StatusCode.DataCorruption, message, messageLocalized);
        }

        // DataCorruption response
        public static OperationResult<T> DataCorruption<T>(
            string message,
            string messageLocalized = APIConstants.APIMessages.DataCorruptionLocalized)
        {
            return new OperationResult<T>(StatusCode.DataCorruption, message, messageLocalized);
        }
    }
}
