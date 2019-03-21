using System.Collections.Generic;
using GreenPipes;
using MassTransit.SmtpGateway.Pipeline.Filters;

namespace MassTransit.Configuration.PipeConfigurators
{
    public sealed class SmtpGatewayPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, ConsumeContext
    {
        readonly SmtpGatewayFilter<TContext> _filter;

        public SmtpGatewayPipeSpecification() => _filter = new SmtpGatewayFilter<TContext>();

        public void Apply(IPipeBuilder<TContext> builder) => builder.AddFilter(_filter);

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}
