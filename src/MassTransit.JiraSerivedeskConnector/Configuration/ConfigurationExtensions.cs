using System;
using GreenPipes;
using MassTransit.JiraSerivedeskConnector.Configuration.PipeConfigurators;

namespace MassTransit.JiraSerivedeskConnector.Configuration
{
    static partial class ConfigurationExtensions
    {
        public static void UseJiraServicedesk<TContext>(this IPipeConfigurator<TContext> configurator,
            Action<IJiraServicedeskConfigurator> configureJiraServicedesk) where TContext : class, PipeContext =>
            configurator.AddPipeSpecification(new JiraServicedeskPipeSpecification<TContext>(configureJiraServicedesk));
    }
}
