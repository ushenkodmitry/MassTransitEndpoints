using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Objects.Commands;

namespace MassTransit.Repositories
{
    public interface ISmtpInstancesRepository
    {
        Task SendCommand(PipeContext context, CreateSmtpInstanceCommand command, CancellationToken cancellationToken = default);
    }
}
