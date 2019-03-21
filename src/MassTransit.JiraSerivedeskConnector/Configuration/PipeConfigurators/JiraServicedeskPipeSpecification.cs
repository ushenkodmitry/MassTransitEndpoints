using System;
using System.Collections.Generic;
using GreenPipes;
using GreenPipes.Validation;
using MassTransit.JiraSerivedeskConnector.Pipeline.Filters;

namespace MassTransit.JiraSerivedeskConnector.Configuration.PipeConfigurators
{
    public sealed class JiraServicedeskPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, PipeContext
    {
        readonly Action<IJiraServicedeskConfigurator> _configureJiraServicedesk;

        readonly JiraServicedeskFilter<TContext> _filter;

        public JiraServicedeskPipeSpecification(Action<IJiraServicedeskConfigurator> configureJiraServicedesk)
        {
            _configureJiraServicedesk = configureJiraServicedesk;

            _filter = new JiraServicedeskFilter<TContext>(configureJiraServicedesk);
        }

        public void Apply(IPipeBuilder<TContext> builder) => builder.AddFilter(_filter);

        public IEnumerable<ValidationResult> Validate()
        {
            var configurator = new JiraServicedeskConfigurator();
            _configureJiraServicedesk(configurator);

            if (string.IsNullOrWhiteSpace(configurator.Options.BaseAddress))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.Options.BaseAddress), "Empty");
            if (string.IsNullOrWhiteSpace(configurator.Options.ConsumerKey))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.Options.ConsumerKey), "Empty");
            if (string.IsNullOrWhiteSpace(configurator.Options.ConsumerSecret))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.Options.ConsumerSecret), "Empty");
        }
    }
}
