using FluentValidation;
using MediatR;
using WeCoreCommon.Behaviours;

namespace WeCoreCommon.Validation.Behaviours;
public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>, IValidateable
where TResponse : class
{
    private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly string _name;
    public ValidationBehaviour(ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
    {
        this._logger = logger;
        this._name = this.GetType().Name;
    }
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehaviour<TRequest, TResponse>> logger):this(logger)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = request.GetType();
        _logger.LogInformation($"Start validate {_name}->{requestName} ");
        var context = new ValidationContext<TRequest>(request);

        var tasks = await Task.WhenAll(_validators.Select(x => x.ValidateAsync(context, cancellationToken)));
        var failures = tasks.SelectMany(x => x.Errors)
            .Where(x => x != null)
            .ToList();
        if (failures.Any())
        {
            _logger.LogInformation($"Validate {_name} Has failures ");
            var responseType = typeof(TResponse);

            if (responseType.IsGenericType)
            {
                var resultType = responseType.GetGenericArguments()[0];
                var invalidResponseType = typeof(HandlerResponse<>).MakeGenericType(resultType);

                var invalidResponse =
                    Activator.CreateInstance(invalidResponseType, null, failures.Select(s => s.ErrorMessage).ToList()) as TResponse;

                return invalidResponse;
            }
        }
        _logger.LogInformation($"Validate {_name} is valid");


        var response = await next();

        return response;
        /*
        var requestName = request.GetType();
        if (validationHandler == null)
        {
            logger.LogInformation("{Request} does not have a validation handler configured.", requestName);
            return await next();
        }

        var result = await validationHandler.Validate(request);
        if (!result.StatusOk)
        {
            logger.LogWarning("Validation failed for {Request}. Error: {Error}", requestName, result.ErrorMessage);
            return new TResponse { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage };
        }

        logger.LogInformation("Validation successful for {Request}.", requestName);
        return await next();*/
    }
}

