using MediatR;

namespace WeCoreCommon.Querying;


public interface IQuery<TResponse> : IRequest<TResponse>
{
}



public interface IPagedQuery<TResponse> : IQuery<TResponse>
{
    int Page { get; }
    int Results { get; }
    string OrderBy { get; }
    string SortOrder { get; }
}