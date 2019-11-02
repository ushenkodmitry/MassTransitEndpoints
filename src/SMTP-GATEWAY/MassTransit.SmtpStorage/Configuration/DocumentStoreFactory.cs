using System;
using Marten;
using MassTransit.Pipeline.Filters;

namespace MassTransit.Configuration
{
    sealed class DocumentStoreFactory : IDocumentStoreFactory
    {
        public IDocumentStore Create(Action<StoreOptions> buildOptions) => DocumentStore.For(buildOptions);
    }
}
