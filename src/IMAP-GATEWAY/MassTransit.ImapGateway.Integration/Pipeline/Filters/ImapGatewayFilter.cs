using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Context;

namespace MassTransit.ImapGateway.Pipeline.Filters
{
    public class ImapGatewayFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        void IProbeSite.Probe(ProbeContext context) => context.CreateFilterScope(nameof(ImapGatewayFilter<TContext>));

        [DebuggerNonUserCode]
        Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            LogContext.Debug?.Log("Sending through filter.");

            throw new NotImplementedException();
        }
    }
}
