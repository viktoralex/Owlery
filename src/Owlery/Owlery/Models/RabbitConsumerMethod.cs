using System;
using System.Reflection;

namespace Owlery.Models
{
    public class RabbitConsumerMethod
    {
        public MethodInfo Method { get; }
        public RabbitConsumerAttribute ConsumerAttributes { get; }
        public RabbitPublisherAttribute PublisherAttributes { get; }
        public Type ParentType { get; }

        public RabbitConsumerMethod(MethodInfo method, Type parentType, RabbitConsumerAttribute consumerAttributes, RabbitPublisherAttribute publisherAttributes)
        {
            this.Method = method;
            this.ConsumerAttributes = consumerAttributes;
            this.PublisherAttributes = publisherAttributes;
            this.ParentType = parentType;
        }
    }
}