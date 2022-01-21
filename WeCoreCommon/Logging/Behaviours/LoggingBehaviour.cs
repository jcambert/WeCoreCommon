using MediatR;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace WeCoreCommon.Logging.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> logger;
    private readonly LoggingOptions options;
    Action<string,  object[] > callbackLogger;

    Func<TRequest, CancellationToken, RequestHandlerDelegate<TResponse>, Task<TResponse>> callbackHandle;
    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger, IOptions<LoggingOptions> options)
    {
        this.logger = logger;
        this.options = options.Value;

        callbackLogger = this.options.LogLevel.ToLower() switch
        {
            "information"=>logger.LogInformation,
            "debug"=>logger.LogDebug,
            "error"=>logger.LogError,
            "trace"=>logger.LogTrace,
            "warning"=>logger.LogWarning,
            _ => logger.LogCritical
        };
        callbackHandle = this.options.UseElapsedTime? HandleWithElapsedTime: HandleWithoutElapsedTime;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var response = await callbackHandle(request, cancellationToken, next);
        if (!string.IsNullOrEmpty(options.Separator.ToString()))
            callbackLogger(new string(options.Separator, options.SeparatorLength), new object[] { });
        return response;
    }

    private async Task<TResponse> HandleWithElapsedTime(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = request.GetType();
        callbackLogger($"{requestName} is starting.", new object[]{} );
        var timer = Stopwatch.StartNew();
        var response = await next();
        timer.Stop();
        callbackLogger($"{requestName} has finished in {timer.ElapsedMilliseconds}ms.", new object[]{} );
        return response;
    }
    private async Task<TResponse> HandleWithoutElapsedTime(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = request.GetType();
        callbackLogger($"{requestName} is starting.", new object[] { });
        var response = await next();
        callbackLogger($"{requestName} has finished ", new object[] { });
        return response;
    }
}