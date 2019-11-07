using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owlery.Models.Settings;
using RabbitMQ.Client;

namespace Owlery.Services
{
    public class DeclarationService : IDeclarationService
    {
        private readonly OwlerySettings settings;
        private readonly ILogger<DeclarationService> logger;

        public DeclarationService(IOptions<OwlerySettings> settings, ILogger<DeclarationService> logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }

        public void DeclareAll(IModel model)
        {
            foreach (var queueSettings in this.settings.Queues)
                this.QueueDeclare(model, queueSettings);

            foreach (var exchangeSettings in this.settings.Exchanges)
                this.ExchangeDeclare(model, exchangeSettings);

            foreach (var bindingSettings in this.settings.Bindings)
                this.QueueBind(model, bindingSettings);
        }

        private void QueueDeclare(IModel model, QueueSettings settings)
        {
            this.logger.LogInformation($"Declaring queue {settings.QueueName}");
            model.QueueDeclare(
                queue: settings.QueueName,
                durable: settings.Durable,
                exclusive: settings.Exclusive,
                autoDelete: settings.AutoDelete,
                arguments: settings.Arguments
            );
        }

        private void ExchangeDeclare(IModel model, ExchangeSettings settings)
        {
            this.logger.LogInformation($"Declaring exchange {settings.ExchangeName}");
            model.ExchangeDeclare(
                exchange: settings.ExchangeName,
                type: settings.Type,
                durable: settings.Durable,
                autoDelete: settings.AutoDelete,
                arguments: settings.Arguments
            );
        }

        private void QueueBind(IModel model, BindingSettings settings)
        {
            this.logger.LogInformation(
                $"Binding queue {settings.QueueName} " +
                $"to exchange {settings.ExchangeName} " +
                $"via routing key {settings.RoutingKey}");
            model.QueueBind(
                queue: settings.QueueName,
                exchange: settings.ExchangeName,
                routingKey: settings.RoutingKey,
                arguments: settings.Arguments
            );
        }
    }
}