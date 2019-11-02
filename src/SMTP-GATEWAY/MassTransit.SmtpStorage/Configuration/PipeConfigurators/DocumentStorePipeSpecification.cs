using System;
using System.Collections.Generic;
using GreenPipes;
using Marten;
using MassTransit.Pipeline.Filters;

namespace MassTransit.Configuration.PipeConfigurators
{
    public sealed class DocumentStorePipeSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : class, PipeContext
    {
        readonly DocumentStoreFilter<TContext> _filter;

        public DocumentStorePipeSpecification(Action<StoreOptions> buildOptions, IDocumentStoreFactory documentStoreFactory)
            => _filter = new DocumentStoreFilter<TContext>(buildOptions, documentStoreFactory);

        void IPipeSpecification<TContext>.Apply(IPipeBuilder<TContext> builder) => builder.AddFilter(_filter);

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }
    }
}
