using System.Collections.Generic;
using Moq;
using Owlery.Models;
using Owlery.Services;
using RabbitMQ.Client;
using Xunit;

namespace Owlery.Tests.Services
{
    public class BasicPropertiesHandler_ApplyMessageProperties
    {
        [Fact]
        public void ShouldApplyNonNullProperties()
        {
            // GIVEN - a message to be applied to message properties
            RabbitMessage message = new RabbitMessage {
                AppId = "AppId",
                Body = "Body",
                ClusterId = "ClusterId",
                ContentEncoding = "ContentEncoding",
                ContentType = "ContentType",
                CorrelationId = "CorrelationId",
                DeliveryMode = 1,
                Expiration = "Expiration",
                Headers = new Dictionary<string, object>() { {"string", new object()} },
                MessageId = "MessageId",
                Persistent = true,
                Priority = 2,
                ReplyTo = "ReplyTo",
                ReplyToAddress = new PublicationAddress("exchangeType", "exchangeName", "routingKey"),
                Timestamp = new AmqpTimestamp(),
                Type = "Type",
                UserId = "UserId",
            };

            Mock<IBasicProperties> mockBasicProperties = new Mock<IBasicProperties>();

            var handler = new BasicPropertiesHandler();

            // WHEN - ApplyMessageProperties is run with the message and a mock basic properties
            var properties = handler.ApplyMessageProperties(message, mockBasicProperties.Object);

            // THEN - Each property of the rabbit message should be applied
            mockBasicProperties.VerifySet(p => p.AppId = message.AppId);
            mockBasicProperties.VerifySet(p => p.ClusterId = message.ClusterId);
            mockBasicProperties.VerifySet(p => p.ContentEncoding = message.ContentEncoding);
            mockBasicProperties.VerifySet(p => p.ContentType = message.ContentType);
            mockBasicProperties.VerifySet(p => p.CorrelationId = message.CorrelationId);
            mockBasicProperties.VerifySet(p => p.DeliveryMode = message.DeliveryMode.Value);
            mockBasicProperties.VerifySet(p => p.Expiration = message.Expiration);
            mockBasicProperties.VerifySet(p => p.Headers = message.Headers);
            mockBasicProperties.VerifySet(p => p.MessageId = message.MessageId);
            mockBasicProperties.VerifySet(p => p.Persistent = message.Persistent.Value);
            mockBasicProperties.VerifySet(p => p.Priority = message.Priority.Value);
            mockBasicProperties.VerifySet(p => p.ReplyTo = message.ReplyTo);
            mockBasicProperties.VerifySet(p => p.ReplyToAddress = message.ReplyToAddress);
            mockBasicProperties.VerifySet(p => p.Timestamp = message.Timestamp.Value);
            mockBasicProperties.VerifySet(p => p.Type = message.Type);
            mockBasicProperties.VerifySet(p => p.UserId = message.UserId);
            mockBasicProperties.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldNotApplyNullProperties()
        {
            // GIVEN - a message to be applied to message properties
            RabbitMessage message = new RabbitMessage {
                AppId = null,
                Body = "Body",
                ClusterId = null,
                ContentEncoding = null,
                ContentType = null,
                CorrelationId = null,
                DeliveryMode = null,
                Expiration = null,
                Headers = null,
                MessageId = null,
                Persistent = null,
                Priority = null,
                ReplyTo = null,
                ReplyToAddress = null,
                Timestamp = null,
                Type = null,
                UserId = null,
            };

            Mock<IBasicProperties> mockBasicProperties = new Mock<IBasicProperties>();

            var handler = new BasicPropertiesHandler();

            // WHEN - ApplyMessageProperties is run with the message and a mock basic properties
            var properties = handler.ApplyMessageProperties(message, mockBasicProperties.Object);

            // THEN - Each property of the rabbit message should be applied
            mockBasicProperties.VerifySet(p => p.AppId = null);
            mockBasicProperties.VerifyGet(p => p.AppId);
            mockBasicProperties.VerifySet(p => p.ClusterId = null);
            mockBasicProperties.VerifyGet(p => p.ClusterId);
            mockBasicProperties.VerifySet(p => p.ContentEncoding = null);
            mockBasicProperties.VerifyGet(p => p.ContentEncoding);
            mockBasicProperties.VerifySet(p => p.ContentType = null);
            mockBasicProperties.VerifyGet(p => p.ContentType);
            mockBasicProperties.VerifySet(p => p.CorrelationId = null);
            mockBasicProperties.VerifyGet(p => p.CorrelationId);
            mockBasicProperties.VerifySet(p => p.DeliveryMode = 0);
            mockBasicProperties.VerifyGet(p => p.DeliveryMode);
            mockBasicProperties.VerifySet(p => p.Expiration = null);
            mockBasicProperties.VerifyGet(p => p.Expiration);
            mockBasicProperties.VerifySet(p => p.Headers = null);
            mockBasicProperties.VerifyGet(p => p.Headers);
            mockBasicProperties.VerifySet(p => p.MessageId = null);
            mockBasicProperties.VerifyGet(p => p.MessageId);
            mockBasicProperties.VerifySet(p => p.Persistent = false);
            mockBasicProperties.VerifyGet(p => p.Persistent);
            mockBasicProperties.VerifySet(p => p.Priority = 0);
            mockBasicProperties.VerifyGet(p => p.Priority);
            mockBasicProperties.VerifySet(p => p.ReplyTo = null);
            mockBasicProperties.VerifyGet(p => p.ReplyTo);
            mockBasicProperties.VerifySet(p => p.ReplyToAddress = null);
            mockBasicProperties.VerifyGet(p => p.ReplyToAddress);
            mockBasicProperties.VerifySet(p => p.Timestamp = new AmqpTimestamp(0));
            mockBasicProperties.VerifyGet(p => p.Timestamp);
            mockBasicProperties.VerifySet(p => p.Type = null);
            mockBasicProperties.VerifyGet(p => p.Type);
            mockBasicProperties.VerifySet(p => p.UserId = null);
            mockBasicProperties.VerifyGet(p => p.UserId);
            mockBasicProperties.VerifyNoOtherCalls();
        }
    }
}