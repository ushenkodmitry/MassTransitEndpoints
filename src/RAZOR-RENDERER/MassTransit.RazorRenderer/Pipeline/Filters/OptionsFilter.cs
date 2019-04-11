using System.Threading.Tasks;
using GreenPipes;
using MassTransit.RazorRenderer.Options;
using MassTransit.RazorRenderer.Contexts;
using System.Diagnostics;

namespace MassTransit.RazorRenderer.Pipeline.Filters
{
    public sealed class OptionsFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        readonly BehaviorOptions _behaviorOptions;

        public OptionsFilter(BehaviorOptions behaviorOptions) => _behaviorOptions = behaviorOptions;

        [DebuggerNonUserCode]
        Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            OptionsContext optionsContext = new ConsumeOptionsContext(_behaviorOptions);

            context.GetOrAddPayload(() => optionsContext);

            return next.Send(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(OptionsFilter<TContext>));
            scope.CreateScope(nameof(BehaviorOptions)).Set(_behaviorOptions);
        }

        sealed class ConsumeOptionsContext : OptionsContext
        {
            public BehaviorOptions BehaviorOptions { get; private set; }

            public ConsumeOptionsContext(BehaviorOptions behaviorOptions) => BehaviorOptions = behaviorOptions;
        }
    }
}