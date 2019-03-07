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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _ = await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => _bus.StopAsync(cancellationToken);
    }
}
