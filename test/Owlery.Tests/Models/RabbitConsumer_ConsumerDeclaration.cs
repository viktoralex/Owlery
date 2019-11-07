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
    public class RabbitConsumer_ConsumerDeclaration
    {
        private readonly ITestOutputHelper output;

        public RabbitConsumer_ConsumerDeclaration(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ShouldStartConsumerWithAutoAck()
        {
            // GIVEN
            var queueName = "queueName";

            Mock<IModel> model = new Mock<IModel>();
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(output);

            RabbitConsumerAttribute consumerAttribute = new RabbitConsumerAttribute(
                queueName,
                acknowledgementType: AcknowledgementType.AutoAck,
                false
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
            model.Verify(m =>
                m.BasicConsume(
                    It.Is<string>(s => s == queueName),
                    It.Is<bool>(b => b == true),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            model.VerifyNoOtherCalls();
        }

        [Fact]
        public void AckOnInvokeShouldStartConsumerWithoutAutoAck()
        {
            // GIVEN
            var queueName = "queueName";

            Mock<IModel> model = new Mock<IModel>();
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(output);

            RabbitConsumerAttribute consumerAttribute = new RabbitConsumerAttribute(
                queueName,
                acknowledgementType: AcknowledgementType.AckOnInvoke,
                false
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
            model.Verify(m =>
                m.BasicConsume(
                    It.Is<string>(s => s == queueName),
                    It.Is<bool>(b => b == false),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            model.VerifyNoOtherCalls();
        }

        [Fact]
        public void AckOnPublishShouldStartConsumerWithoutAutoAck()
        {
            // GIVEN
            var queueName = "queueName";

            Mock<IModel> model = new Mock<IModel>();
            var logger = TestLogger.CreateXUnit<RabbitConsumer>(output);

            RabbitConsumerAttribute consumerAttribute = new RabbitConsumerAttribute(
                queueName,
                acknowledgementType: AcknowledgementType.AckOnPublish,
                false
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
            model.Verify(m =>
                m.BasicConsume(
                    It.Is<string>(s => s == queueName),
                    It.Is<bool>(b => b == false),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<EventingBasicConsumer>()
                )
            );
            model.VerifyNoOtherCalls();
        }
    }
}