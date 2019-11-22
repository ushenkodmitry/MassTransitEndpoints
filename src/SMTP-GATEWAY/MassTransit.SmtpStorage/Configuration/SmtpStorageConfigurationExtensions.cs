using System;
using Marten;
using MassTransit.Configuration.PipeConfigurators;
using MassTransit.Consumers;
using MassTransit.Registries;
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
                .AddPipeSpecification(
                    new DocumentStorePipeSpecification<ConsumeContext>(
                        options =>
                        {
                            options.Connection(smtpStorageConfigurator.ConnectionStringsOptions.SmtpStorage);
                            options.PLV8Enabled = false;
                            options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

                            options.Schema.Include<SmtpConnectionRegistry>();
                            options.Schema.Include<UserCredentialsRegistry>();
                        },
                        new DocumentStoreFactory()));

            busFactoryConfigurator.ReceiveEndpoint("SmtpStorage", endpoint =>
            {
                endpoint.Consumer(() => new CreateSmtpConnectionConsumer(new SmtpConnectionsRepository()));
                endpoint.Consumer(() => new CreateSmtpInstanceConsumer(new SmtpInstancesRepository()));
                endpoint.Consumer(() => new CreateUserCredentialsConsumer(new UserCredentialsRepository()));
            });
        }
    }
}
