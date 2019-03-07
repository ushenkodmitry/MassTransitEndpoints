using Autofac;
using MassTransit;
using MassTransit.NewIdFormatters;
using MassTransit.RabbitMqTransport;
using MassTransit.Util;
using Microsoft.Extensions.Options;
using RabbitMqOptions;

namespace AutofacModules
{
    partial class BusModule
    {
        partial void OverrideDefaultBusEndpointQueueName(IComponentContext componentContext, IRabbitMqBusFactoryConfigurator bus)
        {
            var busOptions = componentContext.Resolve<IOptions<BusOptions>>();

            var queueName = busOptions.Value.QueueNameFormat
                .Replace("{MachineName}", HostMetadataCache.Host.MachineName)
                .Replace("{AssemblyVersion}", HostMetadataCache.Host.AssemblyVersion)
                .Replace("{ProcessName}", HostMetadataCache.Host.ProcessName)
                .Replace("{AssemblyName}", ThisAssembly.GetName().Name)
                .Replace("{NewId}", NewId.Next().ToString(new ZBase32Formatter()));

            bus.OverrideDefaultBusEndpointQueueName(queueName);
        }
    }
}
