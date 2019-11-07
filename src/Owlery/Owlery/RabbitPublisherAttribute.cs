using System;
using System.Collections.Generic;

namespace Owlery
{
    public class RabbitPublisherAttribute : Attribute
    {
        public string RoutingKey { get; }
        public string ExchangeName { get; }

        public RabbitPublisherAttribute(
            string routingKey,
            string exchangeName = "")
        {
            this.RoutingKey = routingKey;
            this.ExchangeName = exchangeName;
        }
    }
}