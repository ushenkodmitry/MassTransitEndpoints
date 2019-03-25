using System;
using GreenPipes;
using MassTransit.RazorRenderer.Configuration.PipeConfigurators;
using MassTransit.RazorRenderer.Consumers;

namespace MassTransit.RazorRenderer.Configuration
{
    public static partial class RazorRendererConfigurationExtensions
    {
        static void UseRenderer<TContext>(this IPipeConfigurator<TContext> configurator,
            Action<IRendererConfigurator> configureRenderer) where TContext : class, ConsumeContext =>
            configurator.AddPipeSpecification(new RendererPipeSpecification<TContext>(configureRenderer));

        public static void UseRazorRenderer(this IReceiveEndpointConfigurator configureEndpoint, Action<IRendererConfigurator> configureRenderer)
        {
            configureEndpoint.UseRenderer(configureRenderer);

            configureEndpoint.Consumer<RenderKeyedTemplateConsumer>();
        }
    }
}
