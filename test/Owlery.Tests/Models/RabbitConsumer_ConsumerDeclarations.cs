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