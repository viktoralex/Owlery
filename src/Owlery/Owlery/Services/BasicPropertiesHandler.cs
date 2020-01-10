using Owlery.Models;
using RabbitMQ.Client;

namespace Owlery.Services
{
    public class BasicPropertiesHandler : IBasicPropertiesHandler
    {
        public IBasicProperties ApplyMessageProperties(RabbitMessage message, IBasicProperties properties)
        {
            properties.AppId = message.AppId ?? properties.AppId;
            properties.ClusterId = message.ClusterId ?? properties.ClusterId;
            properties.ContentEncoding = message.ContentEncoding ?? properties.ContentEncoding;
            properties.ContentType = message.ContentType ?? properties.ContentType;
            properties.CorrelationId = message.CorrelationId ?? properties.CorrelationId;
            properties.DeliveryMode = message.DeliveryMode ?? properties.DeliveryMode;
            properties.Expiration = message.Expiration ?? properties.Expiration;
            properties.Headers = message.Headers ?? properties.Headers;
            properties.MessageId = message.MessageId ?? properties.MessageId;
            properties.Persistent = message.Persistent ?? properties.Persistent;
            properties.Priority = message.Priority ?? properties.Priority;
            properties.ReplyTo = message.ReplyTo ?? properties.ReplyTo;
            properties.ReplyToAddress = message.ReplyToAddress ?? properties.ReplyToAddress;
            properties.Timestamp = message.Timestamp ?? properties.Timestamp;
            properties.Type = message.Type ?? properties.Type;
            properties.UserId = message.UserId ?? properties.UserId;

            return properties;
        }
    }
}