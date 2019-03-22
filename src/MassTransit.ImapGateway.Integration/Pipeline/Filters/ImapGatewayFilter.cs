using GreenPipes;
using MassTransit.Logging;
using System;
using System.Threading.Tasks;

namespace MassTransit.ImapGateway.Pipeline.Filters
{
    public class ImapGatewayFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        static readonly ILog _log = Logger.Get<ImapGatewayFilter<TContext>>();

        public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(ImapGatewayFilter<TContext>));

        public Task Send(TContext context, IPipe<TContext> next)
        {
            _log.Debug(() => "Sending through filter.");

            throw new NotImplementedException();
        }
    }
}
