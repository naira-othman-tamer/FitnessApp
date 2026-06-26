using MediatR;

namespace WorkoutService.Features.Common.Helpers
{
    public interface ICommand<TResponse> : IRequest<TResponse>;
}
