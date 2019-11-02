using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Contexts;
using MassTransit.Objects.Commands;
using MassTransit.Objects.Models;
using MassTransit.Payloads;

namespace MassTransit.Repositories
{
    public sealed class SmtpConnectionsRepository : ISmtpConnectionsRepository
    {
        public async Task SendCommand(PipeContext context, CreateSmtpConnectionCommand command, CancellationToken cancellationToken = default)
        {
            var documentStoreContext = context.GetPayload<DocumentStoreContext>();

            var smtpServer = new SmtpConnection
            {
                Host = command.Host,
                Name = command.Name,
                Port = command.Port,
                UseSsl = command.UseSsl
            };

            using var session = await documentStoreContext.OpenSession(string.Empty).ConfigureAwait(false);

            session.Insert(smtpServer);

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _ = context.AddOrUpdatePayload(
                () => new Identity<SmtpConnection, int>(smtpServer.Id),
                (_) => new Identity<SmtpConnection, int>(smtpServer.Id));
        }
    }
}
