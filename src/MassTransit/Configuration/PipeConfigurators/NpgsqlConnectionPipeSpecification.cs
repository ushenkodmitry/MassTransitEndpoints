using System.Collections.Generic;
using GreenPipes;
using Pipeline.Filters;

namespace Configuration.PipeConfigurators
{
    public sealed class NpgsqlConnectionPipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, PipeContext
    {
        readonly NpgsqlConnectionFilter<TContext> _filter;

        public NpgsqlConnectionPipeSpecification(string connectionString, bool enlistTransaction = true)
            => _filter = new NpgsqlConnectionFilter<TContext>(connectionString, enlistTransaction);

        void IPipeSpecification<TContext>.Apply(IPipeBuilder<TContext> builder) => builder.AddFilter(_filter);

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            yield break;
        }
    }
}
