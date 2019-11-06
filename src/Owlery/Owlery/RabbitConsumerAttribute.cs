using System;
using System.Collections.Generic;

namespace Owlery
{
    public class RabbitConsumerAttribute : Attribute
    {
        public string QueueName { get; }
        public bool Durable { get; }
        public bool Exclusive { get; }
        public bool AutoDelete { get; }
        public IDictionary<string, object> Arguments { get; }

        public RabbitConsumerAttribute(
            string queueName,
            bool durable = false,
            bool exclusive = true,
            bool autoDelete = true,
            object arguments = null)
        {
            this.QueueName = queueName;
            this.Durable = durable;
            this.Exclusive = exclusive;
            this.AutoDelete = autoDelete;
            this.Arguments = (IDictionary<string, object>)arguments;
        }
    }
}