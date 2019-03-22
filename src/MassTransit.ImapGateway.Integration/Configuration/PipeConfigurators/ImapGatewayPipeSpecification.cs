using System.Collections.Generic;
using GreenPipes;

namespace MassTransit.ImapGateway.Configuration.PipeConfigurators
{
    public sealed class ImapPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, ConsumeContext
    {
        public void Apply(IPipeBuilder<TContext> builder)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}
