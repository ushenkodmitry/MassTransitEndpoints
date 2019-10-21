using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Objects.Commands;

namespace MassTransit.Repositories
{
    public interface ISmtpServersRepository
    {
        Task SendCommand(PipeContext context, CreateSmtpServerCommand command, CancellationToken cancellationToken = default);
    }
}
