using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Objects.Commands;

namespace MassTransit.Repositories
{
    public interface ISmtpConnectionsRepository
    {
        Task SendCommand(PipeContext context, CreateSmtpConnectionCommand command, CancellationToken cancellationToken = default);
    }
}
