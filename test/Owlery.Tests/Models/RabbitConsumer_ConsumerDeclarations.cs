using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using Owlery.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;
using Xunit.Abstractions;

namespace Owlery.Tests.Models
{
    public class RabbitConsumer_ConsumerDeclarations
    {
        private readonly ITestOutputHelper output;

        public RabbitConsumer_ConsumerDeclarations(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ShouldDeclareConsumerQueueWithAllAttributes()
        {
            // GIVEN
            var queueName = "queueName";
            var durable = true;
            var exclusive = false;
            var autoDelete = false;
            IDictionary<string, object> args = new Dictionary<string, object>();

            Mock<IModel> model = new Mock<IModel>();
            model.Setup(m =>
                m.QueueDeclare(
                    It.Is<string>(s => s == queueName),
                    It.Is<bool>(b => b == durable),
                    It.Is<bool>(b => b == exclusive),
                    It.Is<bool>(b => b == autoDelete),
                    It.Is<IDictionary<string, object>>(d => d == args)
                )
            );
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(output);

            RabbitConsumerAttribute consumerAttribute = new RabbitConsumerAttribute(
                queueName,
                durable,
                exclusive,
                autoDelete,
                args
            );

            ConsumerMethod method = new ConsumerMethod(
                this.GetType().GetMethods().First(),  // Just use any old method
                this.GetType(),
                consumerAttribute,
                null
            );

            // WHEN
            new RabbitConsumer(method, model.Object, null, logger);

            // THEN
            model.VerifyAll();
        }

        [Fact]
        public void ShouldDeclareDestinationWithAllAttributes()
        {
            // GIVEN
            var exchangeName = "exchange";
            var routingKey = "routingKey";
            var exchangeType = RabbitMQ.Client.ExchangeType.Direct;
            var exchangeDurable = false;
            var exchangeAutoDelete = false;
            var exchangeArguments = new Dictionary<string, object>();
            var destinationQueueName = "destinationQueue";
            var destinationQueueDurable = false;
            var destinationQueueExclusive = true;
            var destinationQueueAutoDelete = true;
            var destinationQueueArguments = new Dictionary<string, object>();
            var bindDestinationQueueAndExchange = true;
            var bindArguments = new Dictionary<string, object>();

            Mock<IModel> model = new Mock<IModel>();
            model.Setup(m =>
                m.ExchangeDeclare(
                    It.Is<string>(s => s == exchangeName),
                    It.Is<string>(s => s == exchangeType),
                    It.Is<bool>(b => b == exchangeDurable),
                    It.Is<bool>(b => b == exchangeAutoDelete),
                    It.Is<IDictionary<string, object>>(d => d == exchangeArguments)
                )
            );
            model.Setup(m =>
                m.QueueDeclare(
                    It.Is<string>(s => s == destinationQueueName),
                    It.Is<bool>(b => b == destinationQueueDurable),
                    It.Is<bool>(b => b == destinationQueueExclusive),
                    It.Is<bool>(b => b == destinationQueueAutoDelete),
                    It.Is<IDictionary<string, object>>(d => d == destinationQueueArguments)
                )
            );
            model.Setup(m =>
                m.QueueBind(
                    It.Is<string>(s => s == destinationQueueName),
                    It.Is<string>(s => s == exchangeName),
                    It.Is<string>(s => s == routingKey),
                    It.Is<IDictionary<string, object>>(d => d == bindArguments)
                )
            );
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(output);

            RabbitConsumerAttribute consumerAttribute = new RabbitConsumerAttribute("consumerQueueName");
            RabbitPublisherAttribute publisherAttribute = new RabbitPublisherAttribute(
                exchangeName, routingKey, exchangeType, exchangeDurable, exchangeAutoDelete,
                exchangeArguments, destinationQueueName, destinationQueueDurable,
                destinationQueueExclusive, destinationQueueAutoDelete, destinationQueueArguments,
                bindDestinationQueueAndExchange, bindArguments
            );

            ConsumerMethod method = new ConsumerMethod(
                this.GetType().GetMethods().First(),  // Just use any old method
                this.GetType(),
                consumerAttribute,
                publisherAttribute
            );

            // WHEN
            new RabbitConsumer(method, model.Object, null, logger);

            // THEN
            model.VerifyAll();
        }

        /* Extension methods (here: IModelExensions.BasicConsume) may not be used in setup / verification expressions.
        [Fact]
        public void ShouldStartConsumer()
        {
            // GIVEN
            var queueName = "queueName";

            Mock<IModel> model = new Mock<IModel>();
            model.Setup(m =>
                m.BasicConsume(
                    It.Is<string>(s => s == queueName),
                    It.Is<bool>(b => b == false),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(output);

            RabbitConsumerAttribute consumerAttribute = new RabbitConsumerAttribute(
                queueName,
                true,
                false,
                false,
                new Dictionary<string, object>()
            );

            ConsumerMethod method = new ConsumerMethod(
                this.GetType().GetMethods().First(),  // Just use any old method
                this.GetType(),
                consumerAttribute,
                null
            );

            // WHEN
            new RabbitConsumer(method, model.Object, null, logger);

            // THEN
            model.VerifyAll();
        }
        */
    }
}