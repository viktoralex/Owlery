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
    public class DeclarationService_QueueArguments
    {
        private readonly ILogger<DeclarationService> logger;
        private readonly Mock<IModel> model;

        public DeclarationService_QueueArguments(ITestOutputHelper output)
        {
            this.logger = TestLogger.CreateXUnit<DeclarationService>(output);
            this.model = new Mock<IModel>();
        }

        [Fact]
        public void ShouldDeclareQueueWithDeadLetterRoutingKeyAndDefaultExchange()
        {
            // GIVEN
            var queueName = "queueName";
            var durable = false;
            var exclusive = false;
            var autoDelete = false;
            var deadLetterRoutingKey = "dlqQueueName";
            var deadLetterExchange = "";

            var expectedArguments = new Dictionary<string, object>();
            expectedArguments.Add(DeclarationService.QUEUE_DEAD_LETTER_ROUTING_KEY_ARGUMENT, deadLetterRoutingKey);
            expectedArguments.Add(DeclarationService.QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT, deadLetterExchange);

            var settings = new OwlerySettings {
                Queues = new Dictionary<string, QueueSettings>() {
                    {
                        "Queue",
                        new QueueSettings() {
                            QueueName = queueName,
                            Durable = durable,
                            Exclusive = exclusive,
                            AutoDelete = autoDelete,

                            DeadLetterRoutingKey = deadLetterRoutingKey,
                        }
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
                    It.Is<IDictionary<string, object>>(d =>
                        deadLetterExchange == (string)d[DeclarationService.QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT] &&
                        deadLetterRoutingKey == (string)d[DeclarationService.QUEUE_DEAD_LETTER_ROUTING_KEY_ARGUMENT]
                    )
                )
            );
            this.model.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldDeclareQueueWithDeadLetterExchange()
        {
            // GIVEN
            var queueName = "queueName";
            var durable = false;
            var exclusive = false;
            var autoDelete = false;
            var deadLetterExchange = "dlqExchange";

            var expectedArguments = new Dictionary<string, object>();
            expectedArguments.Add(DeclarationService.QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT, deadLetterExchange);

            var settings = new OwlerySettings {
                Queues = new Dictionary<string, QueueSettings>() {
                    {
                        "Queue",
                        new QueueSettings() {
                            QueueName = queueName,
                            Durable = durable,
                            Exclusive = exclusive,
                            AutoDelete = autoDelete,

                            DeadLetterExchange = deadLetterExchange,
                        }
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
                    It.Is<IDictionary<string, object>>(d =>
                        deadLetterExchange == (string)d[DeclarationService.QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT]
                    )
                )
            );
            this.model.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldDeclareQueueWithDeadLetterRoutingKeyAndExchange()
        {
            // GIVEN
            var queueName = "queueName";
            var durable = false;
            var exclusive = false;
            var autoDelete = false;
            var deadLetterRoutingKey = "dlqQueueName";
            var deadLetterExchange = "dlqExchange";

            var expectedArguments = new Dictionary<string, object>();
            expectedArguments.Add(DeclarationService.QUEUE_DEAD_LETTER_ROUTING_KEY_ARGUMENT, deadLetterRoutingKey);
            expectedArguments.Add(DeclarationService.QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT, deadLetterExchange);

            var settings = new OwlerySettings {
                Queues = new Dictionary<string, QueueSettings>() {
                    {
                        "Queue",
                        new QueueSettings() {
                            QueueName = queueName,
                            Durable = durable,
                            Exclusive = exclusive,
                            AutoDelete = autoDelete,

                            DeadLetterRoutingKey = deadLetterRoutingKey,
                            DeadLetterExchange = deadLetterExchange,
                        }
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
                    It.Is<IDictionary<string, object>>(d =>
                        deadLetterExchange == (string)d[DeclarationService.QUEUE_DEAD_LETTER_EXCHANGE_ARGUMENT] &&
                        deadLetterRoutingKey == (string)d[DeclarationService.QUEUE_DEAD_LETTER_ROUTING_KEY_ARGUMENT]
                    )
                )
            );
            this.model.VerifyNoOtherCalls();
        }
    }
}