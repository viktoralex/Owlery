using System;
using System.Collections.Generic;

namespace Owlery
{
    public class RabbitConsumerAttribute : Attribute
    {
        public string QueueName { get; }

        public RabbitConsumerAttribute(
            string queueName)
        {
            this.QueueName = queueName;
        }
    }
}