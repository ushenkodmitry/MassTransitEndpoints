using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Objects.Commands;

namespace MassTransit.Repositories
{
    public interface IUserCredentialsRepository
    {
        Task SendCommand(PipeContext context, CreateUserCredentialsCommand command, CancellationToken cancellationToken = default);
    }
}
