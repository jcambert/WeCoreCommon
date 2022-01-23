using MediatR;

namespace WeCoreCommon.Querying;

public abstract class BaseQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
where TQuery : class, IQuery<TResponse>, new()
{
    public BaseQueryHandler()
    {

    }
    public abstract Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken);

}
