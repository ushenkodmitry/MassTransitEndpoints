using System;
using System.Data;
using System.Threading.Tasks;
using GreenPipes;
using Marten;
using MassTransit.Contexts;

namespace MassTransit.Pipeline.Filters
{
    public sealed class DocumentStoreFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IDocumentStore _documentStore;

        public DocumentStoreFilter(Action<StoreOptions> buildOptions, IDocumentStoreFactory documentStoreFactory)
            => _documentStore = documentStoreFactory.Create(buildOptions);

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("documentStore");
        }

        Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            DocumentStoreContext documentStoreContext = new ConsumeDocumentSessionContext(context, _documentStore);

            _ = context.GetOrAddPayload(() => documentStoreContext);

            return next.Send(context);
        }

        sealed class ConsumeDocumentSessionContext : DocumentStoreContext
        {
            readonly IDocumentStore _documentStore;

            public ConsumeDocumentSessionContext(PipeContext context, IDocumentStore documentStore)
                => _documentStore = documentStore;

            ValueTask<IDocumentSession> DocumentStoreContext.OpenSession(string tenantId, IsolationLevel isolationLevel)
            {
                var session = _documentStore.OpenSession(tenantId, isolationLevel: isolationLevel);

                return new ValueTask<IDocumentSession>(session);
            }

            ValueTask<IDocumentSession> DocumentStoreContext.LightweightSession(string tenantId, IsolationLevel isolationLevel)
            {
                var session = _documentStore.LightweightSession(tenantId, isolationLevel);

                return new ValueTask<IDocumentSession>(session);
            }
        }
    }
}
