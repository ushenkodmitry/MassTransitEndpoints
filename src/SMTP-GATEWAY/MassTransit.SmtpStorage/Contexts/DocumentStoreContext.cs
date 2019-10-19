using System.Data;
using System.Threading.Tasks;
using Marten;

namespace MassTransit.Contexts
{
    public interface DocumentStoreContext
    {
        ValueTask<IDocumentSession> OpenSession(string tenantId, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        ValueTask<IDocumentSession> LightweightSession(string tenantId, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
