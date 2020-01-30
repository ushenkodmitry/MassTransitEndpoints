using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Objects.Commands;
using MassTransit.Objects.Models;
using MassTransit.Objects.Queries;

namespace MassTransit.Repositories
{
    public interface ISmtpConnectionsRepository
    {
        Task SendCommand(PipeContext context, CreateSmtpConnectionCommand command, CancellationToken cancellationToken = default);

        Task<SmtpConnection[]> SendQuery(PipeContext context, SmtpConnectionsQuery query, CancellationToken cancellationToken = default);
    }
}
