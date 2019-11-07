using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Owlery.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

        public class ConsumerController
        {
            public bool ConsumeCalled { get; set; }
            public ConsumerController() 
            {
                this.ConsumeCalled = false;
            }

            public void Consume()
            {
                this.ConsumeCalled = true;
            }
        }

        [Fact]
        public void ShouldInvokeConsumer()
        {
            // GIVEN
            var serviceCollectionFactory = new ServiceCollection();
            // Make singleton so we can assert that the method ran
            serviceCollectionFactory.AddSingleton<ConsumerController>();

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
                serviceCollection,
                logger
            );
            rabbitConsumer.RecievedEventHandler(null, eventArgs);

            // THEN
            var service = serviceCollection.GetRequiredService<ConsumerController>();
            Assert.True(service.ConsumeCalled);
        }
    }
}