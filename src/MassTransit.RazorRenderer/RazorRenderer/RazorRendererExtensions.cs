using System;
using MassTransit.RazorRenderer.Configuration;
using MassTransit.RazorRenderer.Configuration.PipeConfigurators;
using MassTransit.RazorRenderer.Consumers;

namespace MassTransit.RazorRenderer
{
    public static class RazorRendererExtensions
    {
        public static void UseRazorRenderer(this IReceiveEndpointConfigurator configureEndpoint, Action<IRendererConfigurator> configureRenderer)
        {
            configureEndpoint.UseRenderer(configureRenderer);
            
            configureEndpoint.Consumer<RenderKeyedTemplateConsumer>();
        }
    }
}