using MediatR;

namespace FCE.Features.Common.Helpers
{
    public interface ICommandRequest<TResponse> : IRequest<TResponse>;
}
