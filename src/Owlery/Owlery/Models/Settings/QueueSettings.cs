using System.Collections.Generic;

namespace Owlery.Models.Settings
{
    public class QueueSettings
    {
        public string QueueName { get; set; }
        public bool Durable { get; set; } = false;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = true;
        public string DeadLetterRoutingKey { get; set; } = null;
        public string DeadLetterExchange { get; set; } = null;
        public IDictionary<string, object> Arguments { get; set; } = null;
    }
}