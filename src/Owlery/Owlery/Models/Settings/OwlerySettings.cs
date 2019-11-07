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

        public List<QueueSettings> Queues { get; set; } = new List<QueueSettings>();
        public List<ExchangeSettings> Exchanges { get; set; } = new List<ExchangeSettings>();
        public List<BindingSettings> Bindings { get; set; } = new List<BindingSettings>();
    }
}