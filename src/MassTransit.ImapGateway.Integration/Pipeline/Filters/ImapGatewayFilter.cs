using GreenPipes;
using System;
using System.Threading.Tasks;

namespace MassTransit.ImapGateway.Pipeline.Filters
{
    public class ImapFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public Task Send(TContext context, IPipe<TContext> next)
        {
            throw new NotImplementedException();
        }
    }
}
