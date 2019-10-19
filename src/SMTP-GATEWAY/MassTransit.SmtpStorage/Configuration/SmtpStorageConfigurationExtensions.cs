using System;
using GreenPipes;
using Marten;
using MassTransit.Configuration.PipeConfigurators;

namespace MassTransit.Configuration
{
    public static partial class SmtpStorageConfigurationExtensions
    { 
        public static void UseSmtpStorage(this IBusFactoryConfigurator busFactoryConfigurator,
            Action<ISmtpStorageConfigurator> configureSmtpStorage = null)
        {
            var smtpStorageConfigurator = new SmtpStorageConfigurator();
            configureSmtpStorage?.Invoke(smtpStorageConfigurator);

            busFactoryConfigurator.UseDocumentStore(storeOptions =>
            {
                storeOptions.Connection(smtpStorageConfigurator.ConnectionStringsOptions.SmtpStorage);
                storeOptions.PLV8Enabled = false;
                storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            });

            busFactoryConfigurator.ReceiveEndpoint("SmtpStorage", endpoint =>
            {
            });
        }

        static void UseDocumentStore<TContext>(this IPipeConfigurator<TContext> configurator, Action<StoreOptions> buildOptions)
            where TContext : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.AddPipeSpecification(new DocumentStorePipeSpecification<TContext>(buildOptions));
        }
    }
}
