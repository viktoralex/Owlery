using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Owlery.HostedServices;
using Owlery.Models.Settings;
using Owlery.Services;

namespace Owlery.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRabbitControllers(this IServiceCollection services)
        {
            // Add RabbitConnection as singleton to allow other services to
            // access it.
            services.AddSingleton<RabbitConnection>();
            services.AddHostedService<BackgroundServiceStarter<RabbitConnection>>();

            services.AddTransient<IDeclarationService, DeclarationService>();
            services.AddTransient<IInvocationParameterService, InvocationParameterService>();
            services.AddTransient<IRabbitService, RabbitService>();

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

        public static void AddRabbitControllers(this IServiceCollection services, IConfigurationSection configSection)
        {
            services.Configure<OwlerySettings>(configSection);

            services.AddRabbitControllers();
        }
    }
}