using System.Collections.Generic;

namespace Owlery.Models.Settings
{
    public class ExchangeSettings
    {
        public string ExchangeName { get; set; }
        public string Type { get; set; } = RabbitMQ.Client.ExchangeType.Direct;
        public bool Durable { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public IDictionary<string, object> Arguments { get; set; } = null;
    }
}