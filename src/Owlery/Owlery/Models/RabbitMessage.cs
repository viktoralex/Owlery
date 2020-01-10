using System.Collections.Generic;
using RabbitMQ.Client;

namespace Owlery.Models
{
    /// <summary>
    /// Message to be published to a RabbitMQ exchange. Any properties with a
    /// value other than null we override the basic properties of the channel.
    /// </summary>
    public class RabbitMessage
    {
        /// <summary>
        /// The body of the message, will be converted to bytes using an
        /// appropriate converter.
        /// </summary>
        public object Body { get; set; }

        public string AppId { get; set; }
        public string ClusterId { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public string CorrelationId { get; set; }
        public byte? DeliveryMode { get; set; }
        public string Expiration { get; set; }
        public IDictionary<string, object> Headers { get; set; }
        public string MessageId { get; set; }
        public bool? Persistent { get; set; }
        public byte? Priority { get; set; }
        public string ReplyTo { get; set; }
        public PublicationAddress ReplyToAddress { get; set; }
        public AmqpTimestamp? Timestamp { get; set; }
        public string Type { get; set; }
        public string UserId { get; set; }

        public RabbitMessage()
        {

        }

        /// <summary>
        /// Create a new message with a body.
        /// </summary>
        /// <param name="body">The body of the message, this will be converted
        /// using an appropriate converter.</param>
        public RabbitMessage(object body)
        {
            this.Body = body;
        }
    }
}