using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Owlery.Utils;
using Owlery.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owlery.Models.Settings;
using Owlery.Services;
using Microsoft.Extensions.Configuration;

namespace Owlery.HostedServices
{
    public class RabbitConnection : IHostedService
    {
        private readonly IDeclarationService declarationService;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;
        private readonly OwlerySettings settings;
        private readonly ILogger logger;
        private readonly ILoggerFactory loggerFactory;

        private IConnection connection;
        private List<RabbitConsumer> consumers;

        public RabbitConnection(
            IDeclarationService declarationService,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IOptions<OwlerySettings> settings,
            ILoggerFactory factory)
        {
            this.declarationService = declarationService;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
            this.settings = settings.Value;
            this.logger = factory.CreateLogger<RabbitConnection>();
            this.loggerFactory = factory;

            this.consumers = new List<RabbitConsumer>();
        }

        ~RabbitConnection(){
            this.connection.Close();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Creating RabbitMQ connection.");

            var factory = RabbitConnectionFactory();
            this.connection = factory.CreateConnection();
            var model = this.connection.CreateModel();

            this.declarationService.DeclareAll(model);

            // Find all controllers and wire them up
            foreach (var consumerMethod in Reflections.GetControllerConsumerMethods())
            {
                this.consumers.Add(
                    new RabbitConsumer(
                        consumerMethod,
                        model,
                        this.configuration,
                        this.serviceProvider,
                        this.loggerFactory.CreateLogger<RabbitConsumer>()));

            }

            return Task.CompletedTask;
        }

        public IModel GetModel()
        {
            return this.connection.CreateModel();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.connection.Close();

            return Task.CompletedTask;
        }

        private ConnectionFactory RabbitConnectionFactory()
        {
            var factory = new ConnectionFactory();

            if (this.settings.UserName != null)
                factory.UserName = this.settings.UserName;

            if (this.settings.Password != null)
                factory.Password = this.settings.Password;

            if (this.settings.VirtualHost != null)
                factory.VirtualHost = this.settings.VirtualHost;

            if (this.settings.HostName != null)
                factory.HostName = this.settings.HostName;

            if (this.settings.Port.HasValue)
                factory.Port = this.settings.Port.Value;

            return factory;
        }


    }
}