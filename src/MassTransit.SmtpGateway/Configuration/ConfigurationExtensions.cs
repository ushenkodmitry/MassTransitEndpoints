using System;
using GreenPipes;
using MassTransit.SmtpGateway.Configuration.PipeConfigurators;

namespace MassTransit.SmtpGateway.Configuration
{
    static partial class ConfigurationExtensions
    {
        public static void UseSmtp<TContext>(this IPipeConfigurator<TContext> configurator,
            Action<ISmtpConfigurator> configureSmtp) where TContext : class, ConsumeContext =>
            configurator.AddPipeSpecification(new SmtpPipeSpecification<TContext>(configureSmtp));
    }
}
