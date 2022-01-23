using System.Text.Json.Serialization;
using WeCoreCommon.Behaviours;

namespace WeCoreCommon.Querying;
public abstract class PagedResultBase<T>:HandlerResponse<T>
    where T : class
{
    public int CurrentPage { get; }
    public int ResultsPerPage { get; }
    public int TotalPages { get; protected set; }
    public long TotalResults { get; protected set; }

    protected PagedResultBase()
    {
    }

    protected PagedResultBase(int currentPage, int resultsPerPage,
        int totalPages, long totalResults)
    {
        CurrentPage = currentPage > totalPages ? totalPages : currentPage;
        ResultsPerPage = resultsPerPage;
        TotalPages = totalPages;
        TotalResults = totalResults;
    }
}

public class PagedResult<T> : PagedResultBase<T>
    where T:class
{
    public IEnumerable<T> Items { get; private set; }

    public bool IsEmpty => Items == null || !Items.Any();
    public bool IsNotEmpty => !IsEmpty;

    protected PagedResult()
    {
        Items = Enumerable.Empty<T>();
    }

    [JsonConstructor]
    protected PagedResult(IEnumerable<T> items,
        int currentPage, int resultsPerPage,
        int totalPages, long totalResults) :
            base(currentPage, resultsPerPage, totalPages, totalResults)
    {
        Items = items;
    }


    public void AddRange(PagedResult<T> range)
    {
        var list = new List<T>();
        list.AddRange(Items);
        list.AddRange(range.Items);
        Items = list;
        TotalPages += range.TotalPages;
        TotalResults += range.TotalResults;
    }
    public static PagedResult<T> Create(IEnumerable<T> items,
        int currentPage, int resultsPerPage,
        int totalPages, long totalResults)
        => new PagedResult<T>(items, currentPage, resultsPerPage, totalPages, totalResults);

    public static PagedResult<T> From(PagedResultBase<T> result, IEnumerable<T> items)
        => new PagedResult<T>(items, result.CurrentPage, result.ResultsPerPage,
            result.TotalPages, result.TotalResults);

    public static PagedResult<T> Empty => new PagedResult<T>();
}