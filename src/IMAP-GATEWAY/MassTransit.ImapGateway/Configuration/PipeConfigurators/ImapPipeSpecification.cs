using GreenPipes;
using GreenPipes.Validation;
using MassTransit.ImapGateway.Options;
using MassTransit.ImapGateway.Pipeline.Filters;
using System;
using System.Collections.Generic;

namespace MassTransit.ImapGateway.Configuration.PipeConfigurators
{
    public sealed class ImapPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, ConsumeContext
    {
        readonly Action<IImapConfigurator> _configureImap;

        public ImapPipeSpecification(Action<IImapConfigurator> configureImap) => _configureImap = configureImap;

        public void Apply(IPipeBuilder<TContext> builder)
        {
            ImapConfigurator imapConfigurator = new ImapConfigurator();
            _configureImap(imapConfigurator);

            var behaviorOptions = imapConfigurator.BehaviorOptions ?? new BehaviorOptions();

            builder.AddFilter(new OptionsFilter<TContext>(imapConfigurator.ServerOptions, behaviorOptions));
            builder.AddFilter(new ImapFilter<TContext>());

            if (behaviorOptions.NoopInterval != default)
                builder.AddFilter(new NoopBehaviorFilter<TContext>(behaviorOptions.NoopInterval));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            ImapConfigurator configurator = new ImapConfigurator();
            _configureImap(configurator);

            if (string.IsNullOrWhiteSpace(configurator.ServerOptions.Host))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.ServerOptions.Host), "Empty");

            if (string.IsNullOrWhiteSpace(configurator.ServerOptions.Username))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.ServerOptions.Username), "Empty");
            else
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Success, nameof(configurator.ServerOptions.Username));

            if (string.IsNullOrWhiteSpace(configurator.ServerOptions.Password))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Failure, nameof(configurator.ServerOptions.Password), "Empty");
            else
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Success, nameof(configurator.ServerOptions.Password));

            if (configurator.BehaviorOptions.FetchInterval == default)
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Warning, nameof(configurator.BehaviorOptions.FetchInterval), "Empty");
            if (configurator.BehaviorOptions.FetchInterval == default)
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Warning, nameof(configurator.BehaviorOptions.NoopInterval), "Empty");
        }
    }
}
