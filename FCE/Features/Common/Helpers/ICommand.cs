using MediatR;

namespace FCE.Features.Common.Helpers
{
    public interface ICommand<TResponse> : IRequest<TResponse>;
}
