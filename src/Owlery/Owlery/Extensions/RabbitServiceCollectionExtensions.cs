using System;
using Microsoft.Extensions.DependencyInjection;
using Owlery.HostedServices;

namespace Owlery.Extensions
{
    public static class RabbitServiceCollectionExtensions
    {
        public static void AddRabbitControllers(this IServiceCollection services)
        {
            services.AddHostedService<RabbitConnection>();

            // Find all controllers and register them as transient services
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsDefined(typeof(RabbitControllerAttribute), false))
                    {
                        services.AddTransient(type);
                    }
                }
            }
        }
    }
}