using System.Dynamic;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Logging;
using MassTransit.RazorRenderer.Contexts;
using RazorLight;

namespace MassTransit.RazorRenderer.Pipeline.Filters
{
    public sealed class RendererFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<RendererFilter<TContext>>();

        public Task Send(TContext context, IPipe<TContext> next)
        {
            _log.Debug(() => "Sending through filter.");
            
            OptionsContext optionsContext = context.GetPayload<OptionsContext>();
            
            RendererContext rendererContext = new ConsumeRendererContext(optionsContext.BehaviorOptions.TemplatesFolder);
            
            context.GetOrAddPayload(() => rendererContext);

            return next.Send(context);
        }

        public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(RendererFilter<TContext>));

        sealed class ConsumeRendererContext : RendererContext
        {
            readonly RazorLightEngine _engine;

            public ConsumeRendererContext(string templateFolder)
            {
                var builder = new RazorLightEngineBuilder().UseMemoryCachingProvider();

                if (string.IsNullOrWhiteSpace(templateFolder))
                    builder = builder.UseFilesystemProject(templateFolder);
                
                _engine = builder.Build();
            }

            public Task<string> CompileRender(string templateKey, object model, ExpandoObject viewBag) => _engine.CompileRenderAsync(templateKey, model, viewBag);
        }
    }
}