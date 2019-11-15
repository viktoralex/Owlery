using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owlery.Models.Settings;
using RabbitMQ.Client;

namespace Owlery.Services
{
    public class DeclarationService : IDeclarationService
    {
        public static string QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT = "x-dead-letter-exchange";
        public static string QUEUE_DEAD_LETTER_ROUTING_KEY_ARGUMENT = "x-dead-letter-routing-key";

        private readonly OwlerySettings settings;
        private readonly ILogger<DeclarationService> logger;

        public DeclarationService(IOptions<OwlerySettings> settings, ILogger<DeclarationService> logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }

        public void DeclareAll(IModel model)
        {
            foreach (var queueSettings in this.settings.Queues.Values)
                this.QueueDeclare(model, queueSettings);

            foreach (var exchangeSettings in this.settings.Exchanges.Values)
                this.ExchangeDeclare(model, exchangeSettings);

            foreach (var bindingSettings in this.settings.Bindings.Values)
                this.QueueBind(model, bindingSettings);
        }

        private void QueueDeclare(IModel model, QueueSettings settings)
        {
            this.logger.LogInformation($"Declaring queue {settings.QueueName}");

            if (settings.Arguments == null)
                settings.Arguments = new Dictionary<string, object>();

            if (settings.DeadLetterRoutingKey != null)
            {
                if (settings.Arguments.Keys.Contains(QUEUE_DEAD_LETTER_ROUTING_KEY_ARGUMENT))
                    settings.Arguments.Remove(QUEUE_DEAD_LETTER_ROUTING_KEY_ARGUMENT);

                settings.Arguments.Add(QUEUE_DEAD_LETTER_ROUTING_KEY_ARGUMENT, settings.DeadLetterRoutingKey);
                // Rabbit will reject routing key only, so default to default exchange
                if (!settings.Arguments.Keys.Contains(QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT))
                    settings.Arguments.Add(QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT, "");
            }

            if (settings.DeadLetterExchange != null)
            {
                if (settings.Arguments.Keys.Contains(QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT))
                    settings.Arguments.Remove(QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT);

                settings.Arguments.Add(QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT, settings.DeadLetterExchange);
            }

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