using System;
using GreenPipes;
using MassTransit.ImapGateway.Configuration.PipeConfigurators;

namespace MassTransit.ImapGateway.Configuration
{
    static partial class ConfigurationExtensions
    {
        public static void UseImap<TContext>(this IPipeConfigurator<TContext> configurator,
            Action<IImapConfigurator> configureSmtp) where TContext : class, ConsumeContext =>
            configurator.AddPipeSpecification(new ImapPipeSpecification<TContext>(configureSmtp));
    }
}
