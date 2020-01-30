using GreenPipes;
using MassTransit.Context;
using MassTransit.ImapGateway.Contexts;
using MassTransit.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.ImapGateway.Pipeline.Filters
{
    public sealed class NoopBehaviorFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        readonly TimeSpan _noopInterval;

        public NoopBehaviorFilter(TimeSpan noopInterval)
        {
            if (noopInterval == default) throw new ArgumentException("Empty", nameof(noopInterval));

            _noopInterval = noopInterval;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(TContext));
            scope.Add(nameof(_noopInterval), _noopInterval);
        }

        [DebuggerNonUserCode]
        async Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            LogContext.Debug?.Log("Sending through filter.");

            using var cts = new CancellationTokenSource();

            ConsumeNoopBehaviorContext noopBehaviorContext = new ConsumeNoopBehaviorContext(context, _noopInterval);

            context.GetOrAddPayload<NoopBehaviorContext>(() => noopBehaviorContext);

            await noopBehaviorContext.ApplyBehavior(cts.Token).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);

            cts.Cancel();
        }

        sealed class ConsumeNoopBehaviorContext : NoopBehaviorContext
        {
            readonly ConsumeContext _context;

            readonly TimeSpan _noopInterval;

            public ConsumeNoopBehaviorContext(ConsumeContext context, TimeSpan noopInterval)
            {
                _context = context;
                _noopInterval = noopInterval;
            }

            public async Task ApplyBehavior(CancellationToken cancellationToken)
            {
                await Task.Yield();

                ImapContext imapContext = _context.GetPayload<ImapContext>();

                while(!cancellationToken.IsCancellationRequested)
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _context.CancellationToken);

                    await Task.Delay(_noopInterval, cts.Token).ConfigureAwait(false);

                    await imapContext.Noop(cts.Token).ConfigureAwait(false);
                }
            }
        }
    }
}
