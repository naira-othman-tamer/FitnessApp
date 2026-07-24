using MediatR;

namespace ProgressTrackingService.Features.Common.Helpers
{
    public interface ICommand<TResponse> : IRequest<TResponse>;
}
