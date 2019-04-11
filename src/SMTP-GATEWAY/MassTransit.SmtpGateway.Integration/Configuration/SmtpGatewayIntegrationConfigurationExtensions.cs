using System;
using MassTransit.SmtpGateway.Configuration.PipeConfigurators;

namespace MassTransit.SmtpGateway.Configuration
{
    public static class SmtpGatewayIntegrationConfigurationExtensions
    {
        public static void UseSmtpGateway(this IConsumePipeConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new SmtpGatewayPipeSpecification<ConsumeContext>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
