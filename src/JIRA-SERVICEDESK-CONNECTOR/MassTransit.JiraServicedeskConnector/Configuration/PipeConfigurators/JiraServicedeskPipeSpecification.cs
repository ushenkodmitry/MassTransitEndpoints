using System;
using System.Collections.Generic;
using GreenPipes;
using GreenPipes.Validation;
using MassTransit.JiraServicedeskConnector.Pipeline.Filters;

namespace MassTransit.JiraServicedeskConnector.Configuration.PipeConfigurators
{
    public sealed class JiraServicedeskPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, ConsumeContext
    {
        readonly Action<IJiraServicedeskConfigurator> _configureJiraServicedesk;

        public JiraServicedeskPipeSpecification(Action<IJiraServicedeskConfigurator> configureJiraServicedesk) => _configureJiraServicedesk = configureJiraServicedesk;

        public void Apply(IPipeBuilder<TContext> builder)
        {
            JiraServicedeskConfigurator configurator = new JiraServicedeskConfigurator();
            _configureJiraServicedesk(configurator);

            if (configurator.BasicAuthOptions != null)
                builder.AddFilter(new JiraBasicAuthFilter<TContext>(configurator.BasicAuthOptions));
            else
                builder.AddFilter(new JiraOAuthFilter<TContext>(configurator.OAuthOptions));

            builder.AddFilter(new JiraServicedeskFilter<TContext>(configurator.ServerOptions, configurator.BehaviorOptions));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            var configurator = new JiraServicedeskConfigurator();
            _configureJiraServicedesk(configurator);

            if (string.IsNullOrWhiteSpace(configurator.ServerOptions.BaseAddress))
                yield return this.Failure(nameof(configurator.ServerOptions.BaseAddress), "Empty");

            if (configurator.BasicAuthOptions == null && configurator.OAuthOptions == null)
            {
                yield return this.Failure(nameof(configurator.BasicAuthOptions), "Empty");
                yield return this.Failure(nameof(configurator.OAuthOptions), "Empty");
            }
            else
            {
                if (configurator.OAuthOptions != null)
                {
                    if (string.IsNullOrWhiteSpace(configurator.OAuthOptions.ConsumerKey))
                        yield return this.Failure(nameof(configurator.OAuthOptions.ConsumerKey), "Empty");

                    if (string.IsNullOrWhiteSpace(configurator.OAuthOptions.ConsumerSecret))
                        yield return this.Failure(nameof(configurator.OAuthOptions.ConsumerSecret), "Empty");
                }
                if(configurator.BasicAuthOptions != null)
                {
                    if (string.IsNullOrWhiteSpace(configurator.BasicAuthOptions.Username))
                        yield return this.Failure(nameof(configurator.BasicAuthOptions.Username), "Empty");

                    if (string.IsNullOrWhiteSpace(configurator.BasicAuthOptions.Password))
                        yield return this.Failure(nameof(configurator.BasicAuthOptions.Password), "Empty");
                }
            }
        }
    }
}
