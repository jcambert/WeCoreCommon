using MediatR;
using System.Reflection;

namespace WeCoreCommon;

public static class ServicesExtensions
{
    //private static 
    internal static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (!services.Any(x => x.ServiceType == typeof(IMediator)))
        {
            var z = new Assembly[assemblies.Length + 1];
            assemblies.CopyTo(z, 1);
            z[0] = typeof(ServicesExtensions).Assembly;
            services.AddMediatR(z);
        }
        return services;
    }
}

