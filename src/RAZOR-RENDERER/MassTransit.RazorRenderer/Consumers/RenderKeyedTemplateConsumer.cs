using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Context;
using MassTransit.RazorRenderer.Contexts;
using MassTransit.RazorRenderer.Messages;
using Newtonsoft.Json;

namespace MassTransit.RazorRenderer.Consumers
{
    sealed class RenderKeyedTemplateConsumer : IConsumer<RenderKeyedTemplate>
    {
        public async Task Consume(ConsumeContext<RenderKeyedTemplate> context)
        {
            object GetModel()
            {
                object model = new object();
                if (!string.IsNullOrWhiteSpace(context.Message.Model))
                    model = JsonConvert.DeserializeObject(context.Message.Model);

                return model;
            }

            LogContext.Debug?.Log($"Rendering of keyed template. Template key: {context.Message.TemplateKey}.");

            var rendererContext = context.GetPayload<RendererContext>();

            var output = await rendererContext.CompileRender(context.Message.TemplateKey, GetModel()).ConfigureAwait(false);

            await context
                .Publish<TemplateRendered>(new
                {
                    context.Message.CorrelationId, 
                    Output = output
                })
                .ConfigureAwait(false);
            
            LogContext.Info?.Log($"Keyed template rendered: Template key: {context.Message.TemplateKey}.");
        }
    }
}
