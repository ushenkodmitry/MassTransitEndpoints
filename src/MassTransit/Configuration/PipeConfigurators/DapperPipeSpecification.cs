using System.Collections.Generic;
using GreenPipes;
using Pipeline.Filters;

namespace Configuration.PipeConfigurators
{
    public sealed class DapperPipeSpecification<TContext> : IPipeSpecification<TContext> 
        where TContext : class, PipeContext
    {
        readonly DapperFilter<TContext> _filter;

        public DapperPipeSpecification() => _filter = new DapperFilter<TContext>();

        void IPipeSpecification<TContext>.Apply(IPipeBuilder<TContext> builder) => builder.AddFilter(_filter);

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            yield break;
        }
    }
}
