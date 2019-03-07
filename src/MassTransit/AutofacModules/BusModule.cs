using Autofac;
using MassTransit;
using MassTransit.AutofacIntegration;
using MassTransit.NLogIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Options;
using RabbitMqOptions;
using System.Linq;
using System.Reflection;
using Module = Autofac.Module;

namespace AutofacModules
{
    public partial class BusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .AddMassTransit(massTransit =>
                {
                    AddConsumers(massTransit, ThisAssembly);

                    massTransit.AddBus(componentContext =>
                    {
                        var hostOptions = componentContext.Resolve<IOptions<HostOptions>>();

                        return Bus.Factory.CreateUsingRabbitMq(bus =>
                        {
                            var host = bus.Host(hostOptions.Value.Host, hostOptions.Value.VirtualHost, rmqHost =>
                            {
                                rmqHost.Username(hostOptions.Value.Username);
                                rmqHost.Password(hostOptions.Value.Password);
                                rmqHost.Heartbeat(hostOptions.Value.Heartbeat);

                                var clusterMembers = hostOptions.Value.ClusterMembers.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();

                                if (clusterMembers.Length > 0)
                                    rmqHost.UseCluster(cluster => cluster.ClusterMembers = clusterMembers);
                            });

                            OverrideDefaultBusEndpointQueueName(componentContext, bus);

                            bus.UseNLog();
                            bus.UseJsonSerializer();
                            UseSerializer(componentContext, bus);
                            UseAutoMapper(componentContext, bus, ThisAssembly);

                            Configure(componentContext, bus, host);
                        });
                    });
                });
        }

        partial void UseSerializer(IComponentContext componentContext, IRabbitMqBusFactoryConfigurator bus);

        partial void UseAutoMapper(IComponentContext componentContext, IRabbitMqBusFactoryConfigurator bus, params Assembly[] assemblies);

        partial void Configure(IComponentContext componentContext, IRabbitMqBusFactoryConfigurator bus, IRabbitMqHost host);

        partial void AddConsumers(IContainerBuilderConfigurator massTransit, params Assembly[] assemblies);

        partial void OverrideDefaultBusEndpointQueueName(IComponentContext componentContext, IRabbitMqBusFactoryConfigurator bus);
    }
}
