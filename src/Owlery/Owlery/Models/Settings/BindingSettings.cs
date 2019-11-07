using System.Collections.Generic;

namespace Owlery.Models.Settings
{
    public class BindingSettings
    {
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
        public IDictionary<string, object> Arguments { get; set; } = null;
    }
}