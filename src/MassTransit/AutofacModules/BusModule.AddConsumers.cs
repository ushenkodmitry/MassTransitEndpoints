using MassTransit;
using MassTransit.AutofacIntegration;
using System.Reflection;

namespace AutofacModules
{
    partial class BusModule
    {
        partial void AddConsumers(IContainerBuilderConfigurator massTransit, params Assembly[] assemblies)
        {
            massTransit.AddConsumers(assemblies);
        }
    }
}
