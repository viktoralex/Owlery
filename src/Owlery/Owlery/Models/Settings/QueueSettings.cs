using System.Collections.Generic;

namespace Owlery.Models.Settings
{
    public class QueueSettings
    {
        public string QueueName { get; set; }
        public bool Durable { get; } = false;
        public bool Exclusive { get; set; } = true;
        public bool autoDelete { get; set; } = true;
        public IDictionary<string, object> Arguments { get; set; } = null;
    }
}