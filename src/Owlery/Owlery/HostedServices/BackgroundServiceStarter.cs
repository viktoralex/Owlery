using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Owlery.HostedServices
{
    /// <summary>
    /// Used to enable access into the hosted services.
    /// https://stackoverflow.com/questions/51254053/how-to-inject-a-reference-to-a-specific-ihostedservice-implementation
    /// </summary>
    public class BackgroundServiceStarter<T> : IHostedService where T:IHostedService
    {
        readonly T backgroundService;

        public BackgroundServiceStarter(T backgroundService)
        {
            this.backgroundService = backgroundService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return backgroundService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return backgroundService.StopAsync(cancellationToken);
        }
    }
}