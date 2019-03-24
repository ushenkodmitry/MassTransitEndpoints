using System;
using GreenPipes;
using MassTransit.RazorRenderer.Configuration.PipeConfigurators;

namespace MassTransit.RazorRenderer.Configuration
{
    static partial class ConfigurationExtensions
    {
        public static void UseRenderer<TContext>(this IPipeConfigurator<TContext> configurator,
            Action<IRendererConfigurator> configureRenderer) where TContext : class, ConsumeContext =>
            configurator.AddPipeSpecification(new RendererPipeSpecification<TContext>(configureRenderer));
    }
}
