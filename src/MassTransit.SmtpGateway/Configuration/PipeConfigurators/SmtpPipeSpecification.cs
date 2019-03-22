using System;
using System.Collections.Generic;
using GreenPipes;
using GreenPipes.Validation;
using MassTransit.SmtpGateway.Options;
using MassTransit.SmtpGateway.Pipeline.Filters;

namespace MassTransit.SmtpGateway.Configuration.PipeConfigurators
{
    public sealed class SmtpPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, ConsumeContext
    {
        readonly Action<ISmtpConfigurator> _configureSmtp;

        public SmtpPipeSpecification(Action<ISmtpConfigurator> configureSmtp) => _configureSmtp = configureSmtp;

        public void Apply(IPipeBuilder<TContext> builder)
        {
            SmtpConfigurator smtpConfigurator = new SmtpConfigurator();
            _configureSmtp(smtpConfigurator);

            var behaviorOptions = smtpConfigurator.BehaviorOptions ?? new BehaviorOptions();

            builder.AddFilter(new OptionsFilter<TContext>(smtpConfigurator.ServerOptions, behaviorOptions));
            builder.AddFilter(new SmtpFilter<TContext>());

            if (behaviorOptions.NoopInterval != default)
                builder.AddFilter(new NoopBehaviorFilter<TContext>(behaviorOptions.NoopInterval));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            var configurator = new SmtpConfigurator();
            _configureSmtp(configurator);

            if (string.IsNullOrWhiteSpace(configurator.ServerOptions.Host))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.ServerOptions.Host), "Empty");
            if (string.IsNullOrWhiteSpace(configurator.ServerOptions.Username))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.ServerOptions.Username), "Empty");
            if (string.IsNullOrWhiteSpace(configurator.ServerOptions.Username))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Warning, nameof(configurator.BehaviorOptions.NoopInterval), "Empty");
        }
    }
}
