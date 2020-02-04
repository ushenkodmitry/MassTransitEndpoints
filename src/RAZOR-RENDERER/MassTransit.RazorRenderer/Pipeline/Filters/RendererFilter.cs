using System.Collections.Concurrent;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Context;
using MassTransit.RazorRenderer.Contexts;
using RazorLight;
using static System.IO.Directory;
using static System.IO.Path;

namespace MassTransit.RazorRenderer.Pipeline.Filters
{
    public sealed class RendererFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        [DebuggerNonUserCode]
        Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            LogContext.Debug?.Log("Sending through filter.");
            
            OptionsContext optionsContext = context.GetPayload<OptionsContext>();

            RendererContext rendererContext = new ConsumeRendererContext(optionsContext.BehaviorOptions.TemplatesFolder);
            
            context.GetOrAddPayload(() => rendererContext);

            return next.Send(context);
        }

        void IProbeSite.Probe(ProbeContext context) => context.CreateFilterScope(nameof(RendererFilter<TContext>));

        sealed class ConsumeRendererContext : RendererContext
        {
            readonly RazorLightEngine _engine;

            readonly ConcurrentDictionary<string, ITemplatePage> _cache;

            public ConsumeRendererContext(string templateFolder)
            {
                _cache = new ConcurrentDictionary<string, ITemplatePage>();

                var builder = new RazorLightEngineBuilder().UseMemoryCachingProvider();

                if (!string.IsNullOrWhiteSpace(templateFolder))
                {
                    if (IsPathRooted(templateFolder))
                        builder = builder.UseFilesystemProject(templateFolder);
                    else
                        builder = builder.UseFilesystemProject(Combine(GetCurrentDirectory(), templateFolder));
                }
                
                _engine = builder.Build();
            }

            public async Task<string> CompileRender(string templateKey, object model, ExpandoObject viewBag)
            {
                var templatePage = _cache.GetOrAdd(templateKey, await _engine.CompileTemplateAsync(templateKey).ConfigureAwait(false));

                return await _engine.RenderTemplateAsync(templatePage, model, viewBag).ConfigureAwait(false);
            }
        }
    }
}
