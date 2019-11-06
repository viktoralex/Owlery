using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Owlery.Utils;
using Owlery.Models;
using Microsoft.Extensions.Logging;

namespace Owlery.HostedServices
{
    public class RabbitConnection : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        private IConnection connection;

        public RabbitConnection(IServiceProvider serviceProvider, ILogger<RabbitConnection> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
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
            foreach (var consumerMethod in RabbitReflections.GetControllerConsumerMethods())
            {
                ConsumerDeclarations(model, consumerMethod.ConsumerAttributes);
                PublisherDeclarations(model, consumerMethod.PublisherAttributes);
                this.logger.LogInformation($"Registering method {consumerMethod.Method.Name} in {consumerMethod.ParentType.Name} as consumer{(consumerMethod.PublisherAttributes == null ? "" : " and publisher")}.");

                var rabbitConsumer = new EventingBasicConsumer(model);
                rabbitConsumer.Received += (ch, ea) => {
                    this.logger.LogInformation($"Received message {ea.DeliveryTag} from {ea.Exchange} with {ea.RoutingKey}. Invoking {consumerMethod.Method.Name} in {consumerMethod.ParentType.Name}.");
                    var body = ea.Body;
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        var consumerService = scope.ServiceProvider.GetRequiredService(consumerMethod.ParentType);

                        var parameters = GetParameterList(consumerMethod, ea, model);
                        var returned = consumerMethod.Method.Invoke(
                            consumerService, parameters);

                        if (consumerMethod.PublisherAttributes != null)
                        {
                            this.logger.LogInformation($"Publishing result of {ea.DeliveryTag} from {consumerMethod.Method.Name} in {consumerMethod.ParentType.Name} to {consumerMethod.PublisherAttributes.ExchangeName} with {consumerMethod.PublisherAttributes.RoutingKey}");
                            model.BasicPublish(
                                consumerMethod.PublisherAttributes.ExchangeName,
                                consumerMethod.PublisherAttributes.RoutingKey,
                                null,
                                PublisherBody(returned));
                        }
                    }
                    // TODO: Acknowledgements
                    // - 1. Auto ack
                    // - 2. Nack if exception
                    // - 3. others?
                    model.BasicAck(ea.DeliveryTag, false);
                };
                model.BasicConsume(consumerMethod.ConsumerAttributes.QueueName, false, rabbitConsumer);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.connection.Close();

            return Task.CompletedTask;
        }

        private void ConsumerDeclarations(IModel model, RabbitConsumerAttribute consumerAttribute)
        {
            this.logger.LogInformation($"Declaring queue {consumerAttribute.QueueName}");
            model.QueueDeclare(
                queue: consumerAttribute.QueueName,
                durable: consumerAttribute.Durable,
                exclusive: consumerAttribute.Exclusive,
                autoDelete: consumerAttribute.AutoDelete,
                arguments: consumerAttribute.Arguments);
        }

        private void PublisherDeclarations(IModel model, RabbitPublisherAttribute publisherAttribute)
        {
            if (publisherAttribute == null)
                return;

            this.logger.LogInformation($"Declaring exchange {publisherAttribute.ExchangeName}");
            model.ExchangeDeclare(
                exchange: publisherAttribute.ExchangeName,
                type: publisherAttribute.ExchangeType,
                durable: publisherAttribute.ExchangeDurable,
                autoDelete: publisherAttribute.ExchangeAutoDelete,
                arguments: publisherAttribute.ExchangeArguments);

            if (publisherAttribute.DestinationQueueName != null)
            {
                this.logger.LogInformation($"Declaring queue {publisherAttribute.DestinationQueueName}");
                model.QueueDeclare(
                    publisherAttribute.DestinationQueueName,
                    publisherAttribute.DestinationQueueDurable,
                    publisherAttribute.DestinationQueueExclusive,
                    publisherAttribute.DestinationQueueAutoDelete,
                    publisherAttribute.DestinationQueueArguments);
            }

            if (publisherAttribute.BindDestinationQueueAndExchange)
            {
                this.logger.LogInformation($"Binding queue {publisherAttribute.DestinationQueueName} to exchange {publisherAttribute.ExchangeName} via routing key {publisherAttribute.RoutingKey}");
                model.QueueBind(
                    publisherAttribute.DestinationQueueName,
                    publisherAttribute.ExchangeName,
                    publisherAttribute.RoutingKey,
                    publisherAttribute.BindArguments);
            }
        }

        private object[] GetParameterList(RabbitConsumerMethod method, BasicDeliverEventArgs eventArgs, IModel model)
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
                        paramList.Add(JsonConvert.DeserializeObject(Encoding.UTF8.GetString(eventArgs.Body), param.ParameterType));
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