using MediatR;
using System.Net;
using WeCoreCommon.Behaviours;

namespace WeCoreCommon.Validation.Behaviours;
public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
where TResponse : HandlerResponse,new()
{
    private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> logger;
    private readonly IValidationHandler<TRequest> validationHandler;
    public ValidationBehaviour(ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }
    public ValidationBehaviour(ILogger<ValidationBehaviour<TRequest, TResponse>> logger, IValidationHandler<TRequest> validationHandler=null):this(logger)
    {
        this.validationHandler = validationHandler;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = request.GetType();
        if (validationHandler == null)
        {
            logger.LogInformation("{Request} does not have a validation handler configured.", requestName);
            return await next();
        }

        var result = await validationHandler.Validate(request);
        if (!result.Ok)
        {
            logger.LogWarning("Validation failed for {Request}. Error: {Error}", requestName, result.ErrorMessage);
            return new TResponse { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        }

        logger.LogInformation("Validation successful for {Request}.", requestName);
        return await next();
    }
}

