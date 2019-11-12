using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Owlery.Models;
using Owlery.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Owlery.Tests.Models
{
    public class RabbitConsumer_RecievedEventHandler
    {
        private readonly ITestOutputHelper output;

        public RabbitConsumer_RecievedEventHandler(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ShouldInvokeConsumer()
        {
            // GIVEN
            var serviceCollectionFactory = new ServiceCollection();
            // Make singleton so we can assert that the method ran
            serviceCollectionFactory.AddSingleton<ConsumerController>();
            serviceCollectionFactory.AddTransient<IByteConversionService, ByteConversionService>();
            serviceCollectionFactory.AddTransient<IInvocationParameterService, InvocationParameterService>();

            var serviceCollection = serviceCollectionFactory.BuildServiceProvider();

            var consumerMethod = new ConsumerMethod(
                typeof(ConsumerController).GetMethod("Consume"),
                typeof(ConsumerController),
                new RabbitConsumerAttribute("consumerQueue"),
                null
            );

            var mockModel = new Mock<IModel>();
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(this.output);

            var eventArgs = new BasicDeliverEventArgs();
            
            // WHEN
            var rabbitConsumer = new RabbitConsumer(
                consumerMethod,
                mockModel.Object,
                null,
                serviceCollection,
                logger
            );
            rabbitConsumer.RecievedEventHandler(null, eventArgs);

            // THEN
            var service = serviceCollection.GetRequiredService<ConsumerController>();
            Assert.True(service.ConsumeCalled);
        }

        [Fact]
        public void ShouldNotAckOrNackOnAutoAcknowledgement()
        {
            // GIVEN - An autoAck consumer
            var serviceCollectionFactory = new ServiceCollection();
            // Make singleton so we can assert that the method ran
            serviceCollectionFactory.AddSingleton<ConsumerController>();
            serviceCollectionFactory.AddTransient<IByteConversionService, ByteConversionService>();
            serviceCollectionFactory.AddTransient<IInvocationParameterService, InvocationParameterService>();

            var serviceCollection = serviceCollectionFactory.BuildServiceProvider();

            var consumerMethod = new ConsumerMethod(
                typeof(ConsumerController).GetMethod("Consume"),
                typeof(ConsumerController),
                new RabbitConsumerAttribute(
                    queueName: "consumerQueue",
                    acknowledgementType: AcknowledgementType.AutoAck),
                null
            );

            var mockModel = new Mock<IModel>();
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(this.output);

            var eventArgs = new BasicDeliverEventArgs();

            // WHEN - The consumer consumes a message
            var rabbitConsumer = new RabbitConsumer(
                consumerMethod,
                mockModel.Object,
                null,
                serviceCollection,
                logger
            );
            rabbitConsumer.RecievedEventHandler(null, eventArgs);

            // THEN - Neither BasicAck nor BasicNack should be called
            var service = serviceCollection.GetRequiredService<ConsumerController>();
            Assert.True(service.ConsumeCalled);

            mockModel.Verify(m => m.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            mockModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldAckOrNackOnManualAcknowledgement()
        {
            // GIVEN - A manual ack consumer
            var serviceCollectionFactory = new ServiceCollection();
            // Make singleton so we can assert that the method ran
            serviceCollectionFactory.AddSingleton<ConsumerController>();
            serviceCollectionFactory.AddTransient<IByteConversionService, ByteConversionService>();
            serviceCollectionFactory.AddTransient<IInvocationParameterService, InvocationParameterService>();

            var serviceCollection = serviceCollectionFactory.BuildServiceProvider();

            var consumerMethod = new ConsumerMethod(
                typeof(ConsumerController).GetMethod("Consume"),
                typeof(ConsumerController),
                new RabbitConsumerAttribute(
                    queueName: "consumerQueue",
                    acknowledgementType: AcknowledgementType.ManualAck),
                null
            );

            var mockModel = new Mock<IModel>();
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(this.output);

            var eventArgs = new BasicDeliverEventArgs();

            // WHEN - The consumer consumes a message
            var rabbitConsumer = new RabbitConsumer(
                consumerMethod,
                mockModel.Object,
                null,
                serviceCollection,
                logger
            );
            rabbitConsumer.RecievedEventHandler(null, eventArgs);

            // THEN - Neither BasicAck nor BasicNack should be called
            var service = serviceCollection.GetRequiredService<ConsumerController>();
            Assert.True(service.ConsumeCalled);

            mockModel.Verify(m => m.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            mockModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldNackOnRabbitMQException()
        {
            // GIVEN - An ack on invoke consumer, throw exception when acking
            ulong deliveryTag = 99;

            var serviceCollectionFactory = new ServiceCollection();
            // Make singleton so we can assert that the method ran
            serviceCollectionFactory.AddSingleton<ConsumerController>();
            serviceCollectionFactory.AddTransient<IByteConversionService, ByteConversionService>();
            serviceCollectionFactory.AddTransient<IInvocationParameterService, InvocationParameterService>();

            var serviceCollection = serviceCollectionFactory.BuildServiceProvider();

            var consumerMethod = new ConsumerMethod(
                typeof(ConsumerController).GetMethod("Consume"),
                typeof(ConsumerController),
                new RabbitConsumerAttribute(
                    queueName: "consumerQueue",
                    acknowledgementType: AcknowledgementType.AckOnInvoke,
                    nackOnException: true),
                null
            );

            var mockModel = new Mock<IModel>();
            mockModel.Setup(
                m => m.BasicAck(
                    It.Is<ulong>(l => l == deliveryTag),
                    It.Is<bool>(b => b == false)
                )
            ).Throws(new AlreadyClosedException(null));
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(this.output);

            var eventArgs = new BasicDeliverEventArgs() {
                DeliveryTag = deliveryTag
            };

            // WHEN - The consumer consumes a message
            var rabbitConsumer = new RabbitConsumer(
                consumerMethod,
                mockModel.Object,
                null,
                serviceCollection,
                logger
            );
            rabbitConsumer.RecievedEventHandler(null, eventArgs);

            // THEN - Ack should be called but it will throw exception so nack will be called
            var service = serviceCollection.GetRequiredService<ConsumerController>();
            Assert.True(service.ConsumeCalled);

            mockModel.Verify(m => m.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            mockModel.Verify(
                m => m.BasicAck(
                    It.Is<ulong>(l => l == deliveryTag),
                    It.Is<bool>(b => b == false)
                )
            );
            mockModel.Verify(m => m.BasicNack(
                It.Is<ulong>(l => l == deliveryTag),
                It.IsAny<bool>(),
                It.IsAny<bool>()
            ));
            mockModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldNackOnInvokcationException()
        {
            // GIVEN - An ack on invoke consumer, throw exception when acking
            ulong deliveryTag = 99;

            var serviceCollectionFactory = new ServiceCollection();
            // Make singleton so we can assert that the method ran
            serviceCollectionFactory.AddSingleton<ConsumerController>(new ConsumerController(throwException: true));
            //serviceCollectionFactory.AddSingleton<ConsumerController>();
            serviceCollectionFactory.AddTransient<IByteConversionService, ByteConversionService>();
            serviceCollectionFactory.AddTransient<IInvocationParameterService, InvocationParameterService>();

            var serviceCollection = serviceCollectionFactory.BuildServiceProvider();

            var consumerMethod = new ConsumerMethod(
                typeof(ConsumerController).GetMethod("Consume"),
                typeof(ConsumerController),
                new RabbitConsumerAttribute(
                    queueName: "consumerQueue",
                    acknowledgementType: AcknowledgementType.AckOnPublish,
                    nackOnException: true),
                null
            );

            var mockModel = new Mock<IModel>();
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(this.output);

            var eventArgs = new BasicDeliverEventArgs() {
                DeliveryTag = deliveryTag
            };

            // WHEN - The consumer consumes a message
            var rabbitConsumer = new RabbitConsumer(
                consumerMethod,
                mockModel.Object,
                null,
                serviceCollection,
                logger
            );
            rabbitConsumer.RecievedEventHandler(null, eventArgs);

            // THEN - Ack should be called but it will throw exception so nack will be called
            var service = serviceCollection.GetRequiredService<ConsumerController>();
            Assert.True(service.ConsumeCalled);

            mockModel.Verify(m => m.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            mockModel.Verify(m => m.BasicNack(
                It.Is<ulong>(l => l == deliveryTag),
                It.IsAny<bool>(),
                It.IsAny<bool>()
            ));
            mockModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldPublishToExchange()
        {
            // GIVEN - A consumer which will publish the returned value
            var routingKey = "routingKey";
            var exchangeName = "exchangeName";

            var serviceCollectionFactory = new ServiceCollection();
            // Make singleton so we can assert that the method ran
            serviceCollectionFactory.AddSingleton<ConsumerController>(new ConsumerController(returnValue: "returnValue"));
            serviceCollectionFactory.AddTransient<IByteConversionService, ByteConversionService>();
            serviceCollectionFactory.AddTransient<IInvocationParameterService, InvocationParameterService>();

            var serviceCollection = serviceCollectionFactory.BuildServiceProvider();

            var consumerMethod = new ConsumerMethod(
                typeof(ConsumerController).GetMethod("Consume"),
                typeof(ConsumerController),
                new RabbitConsumerAttribute(
                    queueName: "consumerQueue",
                    acknowledgementType: AcknowledgementType.AutoAck,
                    nackOnException: true),
                new RabbitPublisherAttribute(
                    routingKey: routingKey,
                    exchangeName: exchangeName
                )
            );

            var mockModel = new Mock<IModel>();
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(this.output);

            var eventArgs = new BasicDeliverEventArgs() {
                DeliveryTag = 99
            };

            // WHEN - The consumer consumes a message
            var rabbitConsumer = new RabbitConsumer(
                consumerMethod,
                mockModel.Object,
                null,
                serviceCollection,
                logger
            );
            rabbitConsumer.RecievedEventHandler(null, eventArgs);

            // THEN - Ack should be called but it will throw exception so nack will be called
            var service = serviceCollection.GetRequiredService<ConsumerController>();
            Assert.True(service.ConsumeCalled);

            mockModel.Verify(m => m.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            mockModel.Verify(m => m.BasicPublish(
                It.Is<string>(s => s == exchangeName),
                It.Is<string>(s => s == routingKey),
                It.IsAny<bool>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<byte[]>()
            ));
            mockModel.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldPublishToConfiguredExchange()
        {
            // GIVEN - A consumer which will publish the returned value
            var routingKeyConfigKey = "Config:Key:For:RoutingKey";
            var routingKeyConfigVal = "routingKey";
            var exchangeNameConfigKey = "Config:Key:For:ExchangeName";
            var exchangeNameConfigVal = "exchangeName";
            var configDict = new Dictionary<string, string>() {
                {routingKeyConfigKey, routingKeyConfigVal},
                {exchangeNameConfigKey, exchangeNameConfigVal}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configDict)
                .Build();

            var serviceCollectionFactory = new ServiceCollection();
            // Make singleton so we can assert that the method ran
            serviceCollectionFactory.AddSingleton<ConsumerController>(new ConsumerController(returnValue: "returnValue"));
            serviceCollectionFactory.AddTransient<IByteConversionService, ByteConversionService>();
            serviceCollectionFactory.AddTransient<IInvocationParameterService, InvocationParameterService>();

            var serviceCollection = serviceCollectionFactory.BuildServiceProvider();

            var consumerMethod = new ConsumerMethod(
                typeof(ConsumerController).GetMethod("Consume"),
                typeof(ConsumerController),
                new RabbitConsumerAttribute(
                    queueName: "consumerQueue",
                    acknowledgementType: AcknowledgementType.AutoAck,
                    nackOnException: true),
                new RabbitPublisherAttribute(
                    routingKey: "{" + routingKeyConfigKey + "}",
                    exchangeName: "{" + exchangeNameConfigKey + "}"
                )
            );

            var mockModel = new Mock<IModel>();
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(this.output);

            var eventArgs = new BasicDeliverEventArgs() {
                DeliveryTag = 99
            };

            // WHEN - The consumer consumes a message
            var rabbitConsumer = new RabbitConsumer(
                consumerMethod,
                mockModel.Object,
                configuration,
                serviceCollection,
                logger
            );
            rabbitConsumer.RecievedEventHandler(null, eventArgs);

            // THEN - Ack should be called but it will throw exception so nack will be called
            var service = serviceCollection.GetRequiredService<ConsumerController>();
            Assert.True(service.ConsumeCalled);

            mockModel.Verify(m => m.BasicConsume(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            mockModel.Verify(m => m.BasicPublish(
                It.Is<string>(s => s == exchangeNameConfigVal),
                It.Is<string>(s => s == routingKeyConfigVal),
                It.IsAny<bool>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<byte[]>()
            ));
            mockModel.VerifyNoOtherCalls();
        }

        public class ConsumerController
        {
            private readonly bool ThrowException;
            public bool ConsumeCalled { get; set; }
            public object ReturnValue { get; set; }

            public ConsumerController()
            {
                this.ConsumeCalled = false;
                this.ThrowException = false;
            }

            public ConsumerController(bool throwException)
            {
                this.ConsumeCalled = false;
                this.ThrowException = throwException;
            }

            public ConsumerController(object returnValue)
            {
                this.ConsumeCalled = false;
                this.ThrowException = false;
                this.ReturnValue = returnValue;
            }

            public object Consume()
            {
                this.ConsumeCalled = true;
                if (this.ThrowException)
                    throw new Exception("An exception");
                return this.ReturnValue;
            }
        }
    }
}