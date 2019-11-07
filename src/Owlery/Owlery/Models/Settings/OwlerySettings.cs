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

        public IEnumerable<QueueSettings> Queues { get; set; }
        public IEnumerable<ExchangeSettings> Exchanges { get; set; }
        public IEnumerable<BindingSettings> Bindings { get; set; }
    }
}