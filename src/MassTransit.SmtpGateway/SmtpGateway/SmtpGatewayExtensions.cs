using System;
using MassTransit.SmtpGateway.Configuration;
using MassTransit.SmtpGateway.Consumers;

namespace MassTransit.SmtpGateway
{
    public static class SmtpGatewayExtensions
    {
        public static void UseSmtpGateway(this IReceiveEndpointConfigurator configureEndpoint, Action<ISmtpConfigurator> configureSmtp)
        {
            configureEndpoint.UseSmtp(configureSmtp);

            configureEndpoint.Consumer<SendMailConsumer>();
        }
    }
}