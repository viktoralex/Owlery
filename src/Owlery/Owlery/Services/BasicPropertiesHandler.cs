using Microsoft.Extensions.Options;
using Owlery.Models;
using Owlery.Models.Settings;
using RabbitMQ.Client;

namespace Owlery.Services
{
    public class BasicPropertiesHandler : IBasicPropertiesHandler
    {
        private readonly OwlerySettings settings;

        public BasicPropertiesHandler(IOptions<OwlerySettings> settings)
        {
            this.settings = settings.Value;
        }

        public void ApplySettingsProperties(IBasicProperties properties)
        {
            if (settings.AppId != null) properties.AppId = settings.AppId;
        }

        public void ApplyMessageProperties(RabbitMessage message, IBasicProperties properties)
        {
            if (message.AppId != null) properties.AppId = message.AppId;
            if (message.ClusterId != null) properties.ClusterId = message.ClusterId;
            if (message.ContentEncoding != null) properties.ContentEncoding = message.ContentEncoding;
            if (message.ContentType != null) properties.ContentType = message.ContentType;
            if (message.CorrelationId != null) properties.CorrelationId = message.CorrelationId;
            if (message.DeliveryMode.HasValue) properties.DeliveryMode = message.DeliveryMode.Value;
            if (message.Expiration != null) properties.Expiration = message.Expiration;
            if (message.Headers != null) properties.Headers = message.Headers;
            if (message.MessageId != null) properties.MessageId = message.MessageId;
            if (message.Persistent.HasValue) properties.Persistent = message.Persistent.Value;
            if (message.Priority.HasValue) properties.Priority = message.Priority.Value;
            if (message.ReplyTo != null) properties.ReplyTo = message.ReplyTo;
            if (message.ReplyToAddress != null) properties.ReplyToAddress = message.ReplyToAddress;
            if (message.Timestamp.HasValue) properties.Timestamp = message.Timestamp.Value;
            if (message.Type != null) properties.Type = message.Type;
            if (message.UserId != null) properties.UserId = message.UserId;
        }
    }
}