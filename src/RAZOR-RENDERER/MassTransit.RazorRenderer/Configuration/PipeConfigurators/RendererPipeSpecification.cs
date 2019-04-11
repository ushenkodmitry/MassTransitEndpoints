using System;
using System.Collections.Generic;
using GreenPipes;
using GreenPipes.Validation;
using MassTransit.RazorRenderer.Pipeline.Filters;

namespace MassTransit.RazorRenderer.Configuration.PipeConfigurators
{
    public sealed class RendererPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, ConsumeContext
    {
        readonly Action<IRendererConfigurator> _configureRenderer;

        public RendererPipeSpecification(Action<IRendererConfigurator> configureRenderer) => _configureRenderer = configureRenderer;

        public void Apply(IPipeBuilder<TContext> builder)
        {
            RendererConfigurator renderConfigurator = new RendererConfigurator();
            _configureRenderer(renderConfigurator);
            
            builder.AddFilter(new OptionsFilter<TContext>(renderConfigurator.BehaviorOptions));
            builder.AddFilter(new RendererFilter<TContext>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            RendererConfigurator renderConfigurator = new RendererConfigurator();
            _configureRenderer(renderConfigurator);
            
            if(string.IsNullOrWhiteSpace(renderConfigurator.BehaviorOptions.TemplatesFolder))
                yield return new ConfigurationValidationResult(ValidationResultDisposition.Warning, nameof(renderConfigurator.BehaviorOptions.TemplatesFolder), "Empty");
        }
    }
}