using System;
using Owlery.Models;

namespace Owlery
{
    public class RabbitConsumerAttribute : Attribute
    {
        public string QueueName { get; }
        public AcknowledgementType AcknowledgementType { get; }
        public bool NackOnException { get; }

        public RabbitConsumerAttribute(
            string queueName,
            AcknowledgementType acknowledgementType = AcknowledgementType.AutoAck,
            bool nackOnException = true)
        {
            this.QueueName = queueName;
            this.AcknowledgementType = acknowledgementType;
            this.NackOnException = nackOnException;
        }
    }
}