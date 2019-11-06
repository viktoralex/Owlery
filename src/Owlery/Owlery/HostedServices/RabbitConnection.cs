using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Owlery.Utils;
using Owlery.Models;
using Microsoft.Extensions.Logging;

namespace Owlery.HostedServices
{
    public class RabbitConnection : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private readonly ILoggerFactory loggerFactory;

        private IConnection connection;
        private List<RabbitConsumer> consumers;

        public RabbitConnection(IServiceProvider serviceProvider, ILoggerFactory factory)
        {
            this.serviceProvider = serviceProvider;
            this.logger = factory.CreateLogger<RabbitConnection>();
            this.loggerFactory = factory;

            this.consumers = new List<RabbitConsumer>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Creating RabbitMQ connection.");

            var factory = new ConnectionFactory() {
                // TODO: customizable connection
            };
            this.connection = factory.CreateConnection();
            var model = this.connection.CreateModel();

            // Find all controllers and wire them up
            foreach (var consumerMethod in Reflections.GetControllerConsumerMethods())
            {
                this.consumers.Add(
                    new RabbitConsumer(
                        consumerMethod,
                        model,
                        this.serviceProvider,
                        this.loggerFactory.CreateLogger<RabbitConsumer>()));

            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.connection.Close();

            return Task.CompletedTask;
        }
    }
}