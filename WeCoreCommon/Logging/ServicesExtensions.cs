using MediatR;
using WeCoreCommon.Logging.Behaviours;

namespace WeCoreCommon.Logging;
public static class ServicesExtensions
{
    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration config, Action<LoggingOptions> configure = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        services.Configure<LoggingOptions>(config.GetSection(LoggingOptions.LogginSectionName));

        LoggingOptions opt = new LoggingOptions();
        configure?.Invoke(opt);
        config.Bind(LoggingOptions.LogginSectionName, opt);


        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        return services;
    }
}
