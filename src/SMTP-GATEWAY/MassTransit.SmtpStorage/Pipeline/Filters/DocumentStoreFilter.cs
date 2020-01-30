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
            _ = context.CreateScope("documentStore");
        }

        Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            DocumentStoreContext documentStoreContext = new ConsumeDocumentSessionContext(_documentStore);

            _ = context.GetOrAddPayload(() => documentStoreContext);

            return next.Send(context);
        }

        sealed class ConsumeDocumentSessionContext : DocumentStoreContext
        {
            readonly IDocumentStore _documentStore;

            public ConsumeDocumentSessionContext(IDocumentStore documentStore)
                => _documentStore = documentStore;

            ValueTask<IDocumentSession> DocumentStoreContext.OpenSession(string tenantId, IsolationLevel isolationLevel) =>
                new ValueTask<IDocumentSession>(_documentStore.OpenSession(tenantId, isolationLevel: isolationLevel));

            ValueTask<IDocumentSession> DocumentStoreContext.LightweightSession(string tenantId, IsolationLevel isolationLevel) =>
                new ValueTask<IDocumentSession>(_documentStore.LightweightSession(tenantId, isolationLevel));

            public ValueTask<IQuerySession> QuerySession(string tenantId) =>
                new ValueTask<IQuerySession>(_documentStore.QuerySession(tenantId));
        }
    }
}
