using MediatR;
using System.Reflection;
using WeCoreCommon.Validation.Behaviours;

namespace WeCoreCommon.Validation;
public static class ServicesExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services, params Assembly[] handlderAssemblies)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        services.AddMediator(handlderAssemblies);
        services.Scan(scan => scan
              .FromAssemblyOf<IValidationHandler>()
                .AddClasses(classes => classes.AssignableTo<IValidationHandler>())
                  .AsImplementedInterfaces()
                  .WithTransientLifetime());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        return services;
    }
}

