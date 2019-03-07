using System;
using System.Collections.Generic;
using GreenPipes;
using GreenPipes.Validation;
using MassTransit.SmtpGateway.Pipeline.Filters;

namespace MassTransit.SmtpGateway.Configuration.PipeConfigurators
{
    public sealed class SmtpPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, PipeContext
    {
        readonly Action<ISmtpConfigurator> _configureSmtp;

        readonly SmtpFilter<TContext> _filter;

        public SmtpPipeSpecification(Action<ISmtpConfigurator> configureSmtp)
        {
            _configureSmtp = configureSmtp;

            _filter = new SmtpFilter<TContext>(configureSmtp);
        }

        public void Apply(IPipeBuilder<TContext> builder) => builder.AddFilter(_filter);

        public IEnumerable<ValidationResult> Validate()
        {
            var configurator = new SmtpConfigurator();
            _configureSmtp(configurator);

            if (string.IsNullOrWhiteSpace(configurator.Options.Host))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.Options.Host), "Empty");
            if (string.IsNullOrWhiteSpace(configurator.Options.Username))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.Options.Username), "Empty");
        }
    }
}
