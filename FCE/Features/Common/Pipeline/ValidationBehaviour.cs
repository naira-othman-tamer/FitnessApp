using FCE.Features.Common.Helpers;
using FluentValidation;
using MediatR;

namespace FCE.Features.Common.Pipeline
{
    public class ValidationBehavior<TRequest, TResponse>
     : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
             TRequest request,
             RequestHandlerDelegate<TResponse> next,
             CancellationToken cancellationToken)
        {

            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var failures = new List<FluentValidation.Results.ValidationFailure>();
            foreach (var validator in _validators)
            {
                var result = await validator.ValidateAsync(context, cancellationToken);

                failures.AddRange(result.Errors.Where(e => e != null));
            }

            if (failures.Any())
            {
                var errorMessage = string.Join(", ", failures.Select(f => f.ErrorMessage));
                var genericType = typeof(TResponse).GetGenericArguments()[0];

                var failureResult = typeof(RequestResult<>)
                    .MakeGenericType(genericType)
                    .GetMethod("Failure")!
                    .Invoke(null, new object[] { errorMessage, RequestErrorCode.ValidationError });

                return (TResponse)failureResult!;
            }

            return await next();
        }
    }
}
