using MediatR;
using ProgressTrackingService.Features.Common.Helpers;
using ProgressTrackingService.Infrastructure;

namespace ProgressTrackingService.Features.Common.Pipeline;

public class TransactionBehavior<TRequest, TResponse>
: IPipelineBehavior<TRequest, TResponse>
where TRequest : ICommand<TResponse>
{
    private readonly UnitOfWork _unitOfWork;

    public TransactionBehavior(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is ISavePoint sp)
        {
            await _unitOfWork.CreateSavePoint(sp.SavePointName, cancellationToken);
        }

        TResponse response = default!;

        await _unitOfWork.ExecuteAsync(async () =>
        {
            response = await next();
        }, cancellationToken);

        return response;
    }
}
