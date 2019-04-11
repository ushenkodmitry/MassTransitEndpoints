using System;
using GreenPipes;
using MassTransit.JiraServicedeskConnector.Configuration.PipeConfigurators;
using MassTransit.JiraServicedeskConnector.Consumers;

namespace MassTransit.JiraServicedeskConnector.Configuration
{
    public static partial class JiraServicedeskConfigurationExtensions
    {
        static void UseServicedesk<TContext>(this IPipeConfigurator<TContext> configurator, Action<IJiraServicedeskConfigurator> configureJiraServicedesk)
            where TContext : class, ConsumeContext => configurator.AddPipeSpecification(new JiraServicedeskPipeSpecification<TContext>(configureJiraServicedesk));

        public static void UseJiraServicedesk(this IReceiveEndpointConfigurator configurator, Action<IJiraServicedeskConfigurator> configureJiraServicedesk)
        {
            configurator.UseServicedesk(configureJiraServicedesk);

            configurator.Consumer<CreateCustomerRequestConsumer>();
        }
    }
}
