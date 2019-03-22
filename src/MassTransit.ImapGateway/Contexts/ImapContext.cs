using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.ImapGateway.Contexts
{
    public interface ImapContext
    {
        Task AuthenticationComplated { get; }

        Task Noop(CancellationToken cancellationToken);
    }
}
