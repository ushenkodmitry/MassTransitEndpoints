using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace HostedServices
{
    public sealed class MassTransitHostedService : IHostedService
    {
        readonly IBusControl _bus;

        public MassTransitHostedService(IBusControl bus) => _bus = bus;

        public Task StartAsync(CancellationToken cancellationToken) => _bus.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _bus.StopAsync(cancellationToken);
    }
}
