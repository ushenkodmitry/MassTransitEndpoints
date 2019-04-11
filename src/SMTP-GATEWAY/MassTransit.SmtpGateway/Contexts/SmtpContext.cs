using System.Threading;
using System.Threading.Tasks;
using MimeKit;

namespace MassTransit.SmtpGateway.Contexts
{
    public interface SmtpContext
    {
        Task AuthenticationCompleted { get; }

        Task Send(MimeMessage message, CancellationToken cancellationToken = default);

        Task Noop(CancellationToken cancellationToken);
    }
}
