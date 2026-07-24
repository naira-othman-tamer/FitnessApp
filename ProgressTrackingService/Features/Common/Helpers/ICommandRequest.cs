using MediatR;

namespace ProgressTrackingService.Features.Common.Helpers
{
    public interface ICommandRequest<TResponse> : IRequest<TResponse>;
}
