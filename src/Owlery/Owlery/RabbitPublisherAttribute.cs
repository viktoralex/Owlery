using System;
using System.Collections.Generic;

namespace Owlery
{
    public class RabbitPublisherAttribute : Attribute
    {
        public string ExchangeName { get; }
        public string RoutingKey { get; }

        public String ExchangeType { get; }
        public bool ExchangeDurable { get; }
        public bool ExchangeAutoDelete { get; }
        public IDictionary<string, object> ExchangeArguments { get; }
        public string DestinationQueueName { get; }
        public bool DestinationQueueDurable { get; }
        public bool DestinationQueueExclusive { get; }
        public bool DestinationQueueAutoDelete { get; }
        public IDictionary<string, object> DestinationQueueArguments { get; }
        public bool BindDestinationQueueAndExchange { get; }
        public IDictionary<string, object> BindArguments { get; }

        public RabbitPublisherAttribute(
            string exchangeName,
            string routingKey,
            String exchangeType = RabbitMQ.Client.ExchangeType.Direct,
            bool exchangeDurable = false,
            bool exchangeAutoDelete = false,
            object exchangeArguments = null,
            string destinationQueueName = null,
            bool destinationQueueDurable = false,
            bool destinationQueueExclusive = true,
            bool destinationQueueAutoDelete = true,
            object destinationQueueArguments = null,
            bool bindDestinationQueueAndExchange = true,
            object bindArguments = null)
        {
            this.ExchangeName = exchangeName;
            this.RoutingKey = routingKey;

            this.ExchangeType = exchangeType;
            this.ExchangeDurable = exchangeDurable;
            this.ExchangeAutoDelete = exchangeAutoDelete;
            this.ExchangeArguments = (IDictionary<string, object>)exchangeArguments;

            this.DestinationQueueName = destinationQueueName;
            this.DestinationQueueDurable = destinationQueueDurable;
            this.DestinationQueueExclusive = destinationQueueExclusive;
            this.DestinationQueueAutoDelete = destinationQueueAutoDelete;
            this.DestinationQueueArguments = (IDictionary<string, object>)destinationQueueArguments;

            if (bindDestinationQueueAndExchange && destinationQueueName == null)
                this.DestinationQueueName = routingKey;

            this.BindDestinationQueueAndExchange = bindDestinationQueueAndExchange;
            this.BindArguments = (IDictionary<string, object>)bindArguments;
        }
    }
}