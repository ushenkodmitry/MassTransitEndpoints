using Autofac;
using MassTransit;

namespace AutofacModules
{
    public sealed partial class ConsumersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterConsumers(ThisAssembly);
        }
    }
}
