using System.Collections.Generic;

namespace Owlery.Models.Settings
{
    public class OwlerySettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string HostName { get; set; }
        public int? Port { get; set; }

        public Dictionary<string, QueueSettings> Queues { get; set; } = new Dictionary<string, QueueSettings>();
        public Dictionary<string, ExchangeSettings> Exchanges { get; set; } = new Dictionary<string, ExchangeSettings>();
        public Dictionary<string, BindingSettings> Bindings { get; set; } = new Dictionary<string, BindingSettings>();
    }
}