using System;
using MassTransit.Configuration.PipeConfigurators;

namespace MassTransit.Configuration
{
    public static class MailGatewayExtensions
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
