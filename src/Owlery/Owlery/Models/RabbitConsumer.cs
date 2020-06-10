using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Owlery.Services;
using Owlery.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Owlery.Models
{
    public class RabbitConsumer
    {
        private readonly ConsumerMethod method;
        private readonly IModel model;
        private readonly IConfiguration configuration;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public RabbitConsumer(
            ConsumerMethod method,
            IModel model,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<RabbitConsumer> logger)
        {
            this.method = method;
            this.model = model;
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
            this.logger = logger;

            this.logger.LogInformation(
                $"Registering method {this.method.Method.Name} in {this.method.ParentType.Name} as consumer " +
                $"of queue {ConsumerQueueName()}.");

            var rabbitConsumer = new EventingBasicConsumer(model);
            rabbitConsumer.Received += this.RecievedEventHandler;

            var autoAck = this.method.ConsumerAttributes.AcknowledgementType == AcknowledgementType.AutoAck;
            model.BasicConsume(
                ConsumerQueueName(),
                autoAck,
                consumerTag: "",
                noLocal: false,
                exclusive: false,
                arguments: null,
                rabbitConsumer);
        }

        public void RecievedEventHandler(object ch, BasicDeliverEventArgs ea)
        {
            this.logger.LogInformation(
                $"Received message {ea.DeliveryTag} from {ea.Exchange} with {ea.RoutingKey}. Invoking " +
                $"{this.method.Method.Name} in {this.method.ParentType.Name}.");

            try
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var parameterConverter = scope.ServiceProvider.GetRequiredService<IInvocationParameterService>();
                    var parameters = parameterConverter.GetParameterList(this.method, ea, model);

                    var consumerService = scope.ServiceProvider.GetRequiredService(this.method.ParentType);
                    var returned = this.method.Method.Invoke(
                        consumerService, parameters);

                    if (this.method.ConsumerAttributes.AcknowledgementType == AcknowledgementType.AckOnInvoke)
                    {
                        this.logger.LogInformation(
                            $"Acknowledging message {ea.DeliveryTag} after invocation.");
                        model.BasicAck(ea.DeliveryTag, false);
                    }

                    if (this.method.PublisherAttributes != null)
                    {
                        var byteConverter = scope.ServiceProvider.GetRequiredService<IByteConversionService>();

                        this.logger.LogInformation(
                            $"Publishing result of {ea.DeliveryTag} from {this.method.Method.Name} in " +
                            $"{this.method.ParentType.Name} to {PublisherExchangeName()} with " +
                            $"{PublisherRoutingKey()}");
                        model.BasicPublish(
                            exchange: PublisherExchangeName(),
                            routingKey: PublisherRoutingKey(),
                            mandatory: true,
                            basicProperties: null,
                            body: byteConverter.ConvertToByteArray(returned));
                    }

                    if (this.method.ConsumerAttributes.AcknowledgementType == AcknowledgementType.AckOnPublish)
                    {
                        this.logger.LogInformation(
                            $"Acknowledging message {ea.DeliveryTag} after publish.");
                        model.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
            catch (Exception exc)
            {
                if (this.method.ConsumerAttributes.NackOnException)
                {
                    this.logger.LogError(
                        exc,
                        $"Message {ea.DeliveryTag} threw exception, will nack.");
                    model.BasicNack(ea.DeliveryTag, false, false);
                }
                else
                {
                    this.logger.LogError(
                        exc,
                        $"Message {ea.DeliveryTag} threw exception.");
                }

                throw;
            }
        }

        private string ConsumerQueueName()
        {
            return ConfigurationFormatter.FormatWithConfig(
                this.method.ConsumerAttributes.QueueName,
                this.configuration);
        }

        private string PublisherExchangeName()
        {
            return ConfigurationFormatter.FormatWithConfig(
                this.method.PublisherAttributes.ExchangeName,
                this.configuration);
        }

        private string PublisherRoutingKey()
        {
            return ConfigurationFormatter.FormatWithConfig(
                this.method.PublisherAttributes.RoutingKey,
                this.configuration);
        }
    }
}