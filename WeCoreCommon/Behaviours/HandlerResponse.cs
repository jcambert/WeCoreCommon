using System.Collections.ObjectModel;
using System.Net;

namespace WeCoreCommon.Behaviours;
public class HandlerResponse
{
    private readonly IList<string> _errorMessages;
    public HandlerResponse(IList<string> errors = null)
    {
        _errorMessages = errors ?? new List<string>();
        this.StatusCode = HttpStatusCode.OK;
    }
    public HttpStatusCode StatusCode { get; init; }
    public string ErrorMessage { get; init; }

    public bool StatusOk => StatusCode == HttpStatusCode.OK;
    public bool IsValidResponse => !_errorMessages.Any() && StatusOk;
    public IReadOnlyCollection<string> Errors => new ReadOnlyCollection<string>(_errorMessages);
    //public static HandlerResponse Success => new HandlerResponse();
    //public static HandlerResponse Fail(string error) => new HandlerResponse { StatusCode =HttpStatusCode.BadRequest, ErrorMessage = error };
}


public class HandlerResponse<TModel> : HandlerResponse
    where TModel : class
{
    public HandlerResponse() : this(default(TModel))
    {

    }
    public HandlerResponse(TModel model, IList<string> validationErrors = null)
        : base(validationErrors)
    {
        Result = model;
    }

    public TModel Result { get; }
}