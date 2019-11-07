using System.Collections.Generic;

namespace Owlery.Models.Settings
{
    public class QueueSettings
    {
        public string QueueName { get; set; }
        public bool Durable { get; set; } = false;
        public bool Exclusive { get; set; } = true;
        public bool AutoDelete { get; set; } = true;
        public IDictionary<string, object> Arguments { get; set; } = null;
    }
}