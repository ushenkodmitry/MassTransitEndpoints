using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Contexts;
using MassTransit.Objects.Commands;
using MassTransit.Objects.Models;
using MassTransit.Objects.Queries;
using MassTransit.Payloads;

namespace MassTransit.Repositories
{
    public sealed class SmtpConnectionsRepository : ISmtpConnectionsRepository
    {
        public async Task SendCommand(PipeContext context, CreateSmtpConnectionCommand command, CancellationToken cancellationToken)
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

            if (context.TryGetCreatedId<SmtpConnection, int>(out var createdId))
                createdId.Id = smtpServer.Id;
        }

        public async Task<SmtpConnection[]> SendQuery(PipeContext context, SmtpConnectionsQuery query, CancellationToken cancellationToken)
        {
            var documentStoreContext = context.GetPayload<DocumentStoreContext>();

            using var session = await documentStoreContext.QuerySession(string.Empty).ConfigureAwait(false);

            var smtpConnections = await session.Query<SmtpConnection>().ToListAsync<SmtpConnection>(cancellationToken).ConfigureAwait(false);

            return smtpConnections.ToArray();
        }
    }
}
