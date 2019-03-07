using Autofac;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Options;
using RabbitMqOptions;

namespace AutofacModules
{
    partial class BusModule
    {
        partial void UseSerializer(IComponentContext componentContext, IRabbitMqBusFactoryConfigurator bus)
        {
            var busOptions = componentContext.Resolve<IOptions<BusOptions>>();

            string serializer = busOptions.Value.Serializer.ToUpper();

            switch (serializer)
            {
                case "json":
                    bus.UseJsonSerializer();
                    break;
                case "bson":
                    bus.UseBsonSerializer();
                    break;
                case "xml":
                    bus.UseXmlSerializer();
                    break;
            }
        }
    }
}
