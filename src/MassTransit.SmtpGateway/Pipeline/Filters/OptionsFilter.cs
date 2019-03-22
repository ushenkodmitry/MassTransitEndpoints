using GreenPipes;
using MassTransit.SmtpGateway.Contexts;
using MassTransit.SmtpGateway.Options;
using System.Threading.Tasks;

namespace MassTransit.SmtpGateway.Pipeline.Filters
{
    public sealed class OptionsFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        readonly ServerOptions _serverOptions;

        readonly BehaviorOptions _behaviorOptions;

        public OptionsFilter(ServerOptions serverOptions, BehaviorOptions behaviorOptions)
        {
            _serverOptions = serverOptions;
            _behaviorOptions = behaviorOptions;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(OptionsFilter<TContext>));
            scope.CreateScope(nameof(ServerOptions)).Set(_serverOptions);
            scope.CreateScope(nameof(BehaviorOptions)).Set(_behaviorOptions);
        }

        public Task Send(TContext context, IPipe<TContext> next)
        {
            OptionsContext optionsContext = new ConsumeOptionsContext(_serverOptions, _behaviorOptions);

            context.GetOrAddPayload(() => optionsContext);

            return next.Send(context);
        }

        sealed class ConsumeOptionsContext : OptionsContext
        {
            public ServerOptions ServerOptions { get; }

            public BehaviorOptions BehaviorOptions { get; }

            public ConsumeOptionsContext(ServerOptions serverOptions, BehaviorOptions behaviorOptions)
            {
                ServerOptions = serverOptions;
                BehaviorOptions = behaviorOptions;
            }
        }
    }
}
