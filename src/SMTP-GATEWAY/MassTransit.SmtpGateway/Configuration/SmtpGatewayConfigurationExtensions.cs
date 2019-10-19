using System;
using MassTransit.SmtpGateway.Configuration.PipeConfigurators;
using MassTransit.SmtpGateway.Consumers;

namespace MassTransit.SmtpGateway.Configuration
{
    public static partial class SmtpGatewayConfigurationExtensions
    {
        static void UseSmtp(this IBusFactoryConfigurator configurator, Action<ISmtpConfigurator> configureSmtp) 
            => configurator.AddPipeSpecification(new SmtpPipeSpecification(configureSmtp));

        public static void UseSmtpGateway(this IBusFactoryConfigurator configureEndpoint, Action<ISmtpConfigurator> configureSmtp)
        {
            configureEndpoint.UseSmtp(configureSmtp);

            //configureEndpoint.Consumer<SendMailConsumer>();
        }
    }
}
