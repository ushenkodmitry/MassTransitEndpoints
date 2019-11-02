using System;
using Marten;

namespace MassTransit.Pipeline.Filters
{
    public interface IDocumentStoreFactory
    {
        IDocumentStore Create(Action<StoreOptions> buildOptions);
    }
}
