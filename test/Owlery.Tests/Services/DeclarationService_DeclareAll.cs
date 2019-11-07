using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Owlery.Models.Settings;
using Owlery.Services;
using RabbitMQ.Client;
using Xunit;
using Xunit.Abstractions;

namespace Owlery.Tests.Services
{
    public class DeclarationService_DeclareAll
    {
        private readonly ILogger<DeclarationService> logger;
        private readonly Mock<IModel> model;

        public DeclarationService_DeclareAll(ITestOutputHelper output)
        {
            this.logger = TestLogger.CreateXUnit<DeclarationService>(output);
            this.model = new Mock<IModel>();
        }

        [Fact]
        public void ShouldDeclareQueuesWithAttributes()
        {
            // GIVEN
            var queueName = "queueName";
            var durable = false;
            var exclusive = false;
            var autoDelete = false;
            var arguments = new Dictionary<string, object>();

            var settings = new OwlerySettings {
                Queues = new List<QueueSettings>() {
                    new QueueSettings() {
                        QueueName = queueName,
                        Durable = durable,
                        Exclusive = exclusive,
                        AutoDelete = autoDelete,
                        Arguments = arguments,
                    }
                }
            };

            var service = new DeclarationService(
                Options.Create<OwlerySettings>(settings),
                this.logger);

            // WHEN
            service.DeclareAll(this.model.Object);

            // THEN
            this.model.Verify(
                m => m.QueueDeclare(
                    It.Is<string>(s => s == queueName),
                    It.Is<bool>(b => b == durable),
                    It.Is<bool>(b => b == exclusive),
                    It.Is<bool>(b => b == autoDelete),
                    It.Is<IDictionary<string, object>>(d => d == arguments)
                )
            );
            this.model.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldDeclareExchangesWithAttributes()
        {
            // GIVEN
            var exchangeName = "exchangeName";
            var type = "exchangeType";
            var durable = false;
            var autoDelete = false;
            var arguments = new Dictionary<string, object>();

            var settings = new OwlerySettings {
                Exchanges = new List<ExchangeSettings>() {
                    new ExchangeSettings() {
                        ExchangeName = exchangeName,
                        Type = type,
                        Durable = durable,
                        AutoDelete = autoDelete,
                        Arguments = arguments,
                    }
                }
            };

            var service = new DeclarationService(
                Options.Create<OwlerySettings>(settings),
                this.logger);

            // WHEN
            service.DeclareAll(this.model.Object);

            // THEN
            this.model.Verify(
                m => m.ExchangeDeclare(
                    It.Is<string>(s => s == exchangeName),
                    It.Is<string>(s => s == type),
                    It.Is<bool>(b => b == durable),
                    It.Is<bool>(b => b == autoDelete),
                    It.Is<IDictionary<string, object>>(d => d == arguments)
                )
            );
            this.model.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldBindQueueToExchangeWithAttributes()
        {
            // GIVEN
            var queueName = "exchangeName";
            var exchangeName = "exchangeName";
            var routingKey = "routingKey";
            var arguments = new Dictionary<string, object>();

            var settings = new OwlerySettings {
                Bindings = new List<BindingSettings>() {
                    new BindingSettings() {
                        QueueName = queueName,
                        ExchangeName = exchangeName,
                        RoutingKey = routingKey,
                        Arguments = arguments,
                    }
                }
            };

            var service = new DeclarationService(
                Options.Create<OwlerySettings>(settings),
                this.logger);

            // WHEN
            service.DeclareAll(this.model.Object);

            // THEN
            this.model.Verify(
                m => m.QueueBind(
                    It.Is<string>(s => s == queueName),
                    It.Is<string>(s => s == exchangeName),
                    It.Is<string>(s => s == routingKey),
                    It.Is<IDictionary<string, object>>(d => d == arguments)
                )
            );
            this.model.VerifyNoOtherCalls();
        }
    }
}