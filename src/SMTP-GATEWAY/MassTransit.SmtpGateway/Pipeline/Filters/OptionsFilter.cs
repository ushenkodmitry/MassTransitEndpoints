using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Context;
using MassTransit.SmtpGateway.Contexts;
using MassTransit.SmtpGateway.Options;

namespace MassTransit.SmtpGateway.Pipeline.Filters
{
    public sealed class OptionsFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly ServerOptions _serverOptions;

        readonly BehaviorOptions _behaviorOptions;

        public OptionsFilter(ServerOptions serverOptions, BehaviorOptions behaviorOptions)
        {
            _serverOptions = serverOptions;
            _behaviorOptions = behaviorOptions;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(OptionsFilter<TContext>));
            scope.CreateScope(nameof(ServerOptions)).Set(_serverOptions);
            scope.CreateScope(nameof(BehaviorOptions)).Set(_behaviorOptions);
        }

        [DebuggerNonUserCode]
        Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            LogContext.Debug?.Log("Sending through filter.");

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
