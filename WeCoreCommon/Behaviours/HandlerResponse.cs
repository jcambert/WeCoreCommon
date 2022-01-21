using System.Net;

namespace WeCoreCommon.Behaviours;
public class HandlerResponse
{
    public HandlerResponse()
    {
        this.StatusCode =HttpStatusCode.OK;
    }
    public HttpStatusCode StatusCode { get; init; }
    public string ErrorMessage { get; init; }

    public bool Ok => StatusCode == HttpStatusCode.OK;

    public static HandlerResponse Success => new HandlerResponse();
    public static HandlerResponse Fail(string error) => new HandlerResponse { StatusCode =HttpStatusCode.BadRequest, ErrorMessage = error };
}