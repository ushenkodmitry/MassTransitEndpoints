using System;
using GreenPipes;
using Marten;
using MassTransit.Configuration.PipeConfigurators;
using MassTransit.Consumers;
using MassTransit.Repositories;

namespace MassTransit.Configuration
{
    public static partial class SmtpStorageConfigurationExtensions
    { 
        public static void UseSmtpStorage(this IBusFactoryConfigurator busFactoryConfigurator,
            Action<ISmtpStorageConfigurator> configureSmtpStorage = null)
        {
            var smtpStorageConfigurator = new SmtpStorageConfigurator();
            configureSmtpStorage?.Invoke(smtpStorageConfigurator);

            busFactoryConfigurator
                .AddPipeSpecification(new DocumentStorePipeSpecification<ConsumeContext>(storeOptions =>
                {
                    storeOptions.Connection(smtpStorageConfigurator.ConnectionStringsOptions.SmtpStorage);
                    storeOptions.PLV8Enabled = false;
                    storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                }));

            busFactoryConfigurator.ReceiveEndpoint("SmtpStorage", endpoint =>
            {
                endpoint.Consumer(() => new CreateSmtpServerConsumer(new SmtpServersRepository()));
            });
        }
    }
}
