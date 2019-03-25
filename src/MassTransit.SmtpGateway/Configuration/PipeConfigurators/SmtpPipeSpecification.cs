using System;
using System.Collections.Generic;
using GreenPipes;
using GreenPipes.Validation;
using MassTransit.SmtpGateway.Options;
using MassTransit.SmtpGateway.Pipeline.Filters;

namespace MassTransit.SmtpGateway.Configuration.PipeConfigurators
{
    public sealed class SmtpPipeSpecification : IPipeSpecification<ConsumeContext>
    {
        readonly Action<ISmtpConfigurator> _configureSmtp;

        public SmtpPipeSpecification(Action<ISmtpConfigurator> configureSmtp) => _configureSmtp = configureSmtp;

        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            SmtpConfigurator smtpConfigurator = new SmtpConfigurator();
            _configureSmtp(smtpConfigurator);

            var behaviorOptions = smtpConfigurator.BehaviorOptions ?? new BehaviorOptions();

            builder.AddFilter(new OptionsFilter<ConsumeContext>(smtpConfigurator.ServerOptions, behaviorOptions));
            builder.AddFilter(new SmtpFilter<ConsumeContext>());

            if (behaviorOptions.NoopInterval != default)
                builder.AddFilter(new NoopBehaviorFilter<ConsumeContext>(behaviorOptions.NoopInterval));
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
