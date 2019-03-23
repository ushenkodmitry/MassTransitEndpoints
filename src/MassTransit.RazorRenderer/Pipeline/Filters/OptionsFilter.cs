using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Options;
using MassTransit.RazorRenderer.Contexts;

namespace MassTransit.RazorRenderer.Pipeline.Filters
{
    public sealed class OptionsFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        readonly BehaviorOptions _behaviorOptions;

        public OptionsFilter(BehaviorOptions behaviorOptions) => _behaviorOptions = behaviorOptions;

        public Task Send(TContext context, IPipe<TContext> next)
        {
            OptionsContext optionsContext = new ConsumeOptionsContext(_behaviorOptions);

            context.GetOrAddPayload(() => optionsContext);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
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