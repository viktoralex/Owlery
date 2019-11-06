using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Owlery.Models
{
    public class RabbitConsumer
    {
        private readonly ConsumerMethod method;
        private readonly IModel model;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public RabbitConsumer(
            ConsumerMethod method,
            IModel model,
            IServiceProvider serviceProvider,
            ILogger<RabbitConsumer> logger)
        {
            this.method = method;
            this.model = model;
            this.serviceProvider = serviceProvider;
            this.logger = logger;

            this.ConsumerDeclarations();
            this.PublisherDeclarations();
            this.logger.LogInformation(
                $"Registering method {this.method.Method.Name} in {this.method.ParentType.Name} as consumer" +
                $"{(this.method.PublisherAttributes == null ? "" : " and publisher")}.");

            var rabbitConsumer = new EventingBasicConsumer(model);
            rabbitConsumer.Received += this.RecievedEventHandler;

            model.BasicConsume(this.method.ConsumerAttributes.QueueName, false, rabbitConsumer);
        }

        public void RecievedEventHandler(object ch, BasicDeliverEventArgs ea)
        {
            this.logger.LogInformation(
                $"Received message {ea.DeliveryTag} from {ea.Exchange} with {ea.RoutingKey}. Invoking " +
                $"{this.method.Method.Name} in {this.method.ParentType.Name}.");
            var body = ea.Body;
            using (var scope = this.serviceProvider.CreateScope())
            {
                var consumerService = scope.ServiceProvider.GetRequiredService(this.method.ParentType);

                var parameters = GetParameterList(this.method, ea, model);
                var returned = this.method.Method.Invoke(
                    consumerService, parameters);

                if (this.method.PublisherAttributes != null)
                {
                    this.logger.LogInformation(
                        $"Publishing result of {ea.DeliveryTag} from {this.method.Method.Name} in " +
                        $"{this.method.ParentType.Name} to {this.method.PublisherAttributes.ExchangeName} with " +
                        $"{this.method.PublisherAttributes.RoutingKey}");
                    model.BasicPublish(
                        this.method.PublisherAttributes.ExchangeName,
                        this.method.PublisherAttributes.RoutingKey,
                        null,
                        PublisherBody(returned));
                }
            }
            // TODO: Acknowledgements
            // - 1. Auto ack
            // - 2. Nack if exception
            // - 3. others?
            model.BasicAck(ea.DeliveryTag, false);
        }

        private void ConsumerDeclarations()
        {
            this.logger.LogInformation($"Declaring queue {this.method.ConsumerAttributes.QueueName}");
            this.model.QueueDeclare(
                queue: this.method.ConsumerAttributes.QueueName,
                durable: this.method.ConsumerAttributes.Durable,
                exclusive: this.method.ConsumerAttributes.Exclusive,
                autoDelete: this.method.ConsumerAttributes.AutoDelete,
                arguments: this.method.ConsumerAttributes.Arguments);
        }

        private void PublisherDeclarations()
        {
            if (this.method.PublisherAttributes == null)
                return;

            this.logger.LogInformation($"Declaring exchange {this.method.PublisherAttributes.ExchangeName}");
            model.ExchangeDeclare(
                exchange: this.method.PublisherAttributes.ExchangeName,
                type: this.method.PublisherAttributes.ExchangeType,
                durable: this.method.PublisherAttributes.ExchangeDurable,
                autoDelete: this.method.PublisherAttributes.ExchangeAutoDelete,
                arguments: this.method.PublisherAttributes.ExchangeArguments);

            if (this.method.PublisherAttributes.DestinationQueueName != null)
            {
                this.logger.LogInformation($"Declaring queue {this.method.PublisherAttributes.DestinationQueueName}");
                model.QueueDeclare(
                    this.method.PublisherAttributes.DestinationQueueName,
                    this.method.PublisherAttributes.DestinationQueueDurable,
                    this.method.PublisherAttributes.DestinationQueueExclusive,
                    this.method.PublisherAttributes.DestinationQueueAutoDelete,
                    this.method.PublisherAttributes.DestinationQueueArguments);
            }

            if (this.method.PublisherAttributes.BindDestinationQueueAndExchange)
            {
                this.logger.LogInformation(
                    $"Binding queue {this.method.PublisherAttributes.DestinationQueueName} to exchange " +
                    $"{this.method.PublisherAttributes.ExchangeName} via routing key " +
                    $"{this.method.PublisherAttributes.RoutingKey}");
                model.QueueBind(
                    this.method.PublisherAttributes.DestinationQueueName,
                    this.method.PublisherAttributes.ExchangeName,
                    this.method.PublisherAttributes.RoutingKey,
                    this.method.PublisherAttributes.BindArguments);
            }
        }

        private object[] GetParameterList(ConsumerMethod method, BasicDeliverEventArgs eventArgs, IModel model)
        {
            List<object> paramList = new List<object>();
            foreach (var param in method.Method.GetParameters())
            {
                if (param.IsDefined(typeof(FromBodyAttribute), false))
                {
                    if (param.ParameterType == typeof(byte[]))
                    {
                        paramList.Add(eventArgs.Body);
                    }
                    else if (param.ParameterType == typeof(string))
                    {
                        paramList.Add(Encoding.UTF8.GetString(eventArgs.Body));
                    }
                    else
                    {
                        // TODO: other converters
                        paramList.Add(
                            JsonConvert.DeserializeObject(
                                Encoding.UTF8.GetString(eventArgs.Body),
                                param.ParameterType));
                    }
                }
                else if (param.IsDefined(typeof(FromDeliveryTagAttribute), false))
                {
                    paramList.Add(eventArgs.DeliveryTag);
                }
                else if (param.IsDefined(typeof(FromModelAttribute), false))
                {
                    paramList.Add(model);
                }
                else if (param.IsDefined(typeof(FromBasicPropertiesAttribute), false))
                {
                    paramList.Add(eventArgs.BasicProperties);
                }
                else if (param.IsDefined(typeof(FromConsumerTagAttribute), false))
                {
                    paramList.Add(eventArgs.ConsumerTag);
                }
                else if (param.IsDefined(typeof(FromExchangeAttribute), false))
                {
                    paramList.Add(eventArgs.Exchange);
                }
                else if (param.IsDefined(typeof(FromRedeliveredAttribute), false))
                {
                    paramList.Add(eventArgs.Redelivered);
                }
                else if (param.IsDefined(typeof(FromRoutingKeyAttribute), false))
                {
                    paramList.Add(eventArgs.RoutingKey);
                }
            }

            return paramList.ToArray();
        }

        private byte[] PublisherBody(object returned)
        {
            if (returned.GetType() == typeof(byte[]))
            {
                return (byte[])returned;
            }
            else if (returned.GetType() == typeof(string))
            {
                return Encoding.UTF8.GetBytes((string)returned);
            }

            // TODO: other converters
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(returned));
        }
    }
}