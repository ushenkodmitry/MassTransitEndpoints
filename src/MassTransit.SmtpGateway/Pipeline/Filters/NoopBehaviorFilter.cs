using GreenPipes;
using MassTransit.ImapGateway.Contexts;
using MassTransit.Logging;
using MassTransit.SmtpGateway.Contexts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.SmtpGateway.Pipeline.Filters
{
    public sealed class NoopBehaviorFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        static readonly ILog _log = Logger.Get<NoopBehaviorFilter<TContext>>();

        readonly TimeSpan _noopInterval;

        public NoopBehaviorFilter(TimeSpan noopInterval)
        {
            if (noopInterval == default) throw new ArgumentException("Empty", nameof(noopInterval));

            _noopInterval = noopInterval;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(TContext));
            scope.Add(nameof(_noopInterval), _noopInterval);
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            _log.Debug(() => "Sending through filter.");

            using (var cts = new CancellationTokenSource())
            {
                ConsumeNoopBehaviorContext noopBehaviorContext = new ConsumeNoopBehaviorContext(context, _noopInterval);

                context.GetOrAddPayload<NoopBehaviorContext>(() => noopBehaviorContext);

                await noopBehaviorContext.ApplyBehavior(cts.Token).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);

                cts.Cancel();
            }
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

                SmtpContext imapContext = _context.GetPayload<SmtpContext>();

                while(!cancellationToken.IsCancellationRequested)
                {
                    using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _context.CancellationToken))
                    {
                        await Task.Delay(_noopInterval, cts.Token).ConfigureAwait(false);

                        await imapContext.Noop(cts.Token).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
