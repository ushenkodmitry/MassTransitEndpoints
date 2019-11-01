using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Contexts;
using MassTransit.Objects.Commands;
using MassTransit.Objects.Models;
using MassTransit.Payloads;

namespace MassTransit.Repositories
{
    public sealed class UserCredentialsRepository : IUserCredentialsRepository
    {
        public async Task SendCommand(PipeContext context, CreateUserCredentialsCommand command, CancellationToken cancellationToken = default)
        {
            var documentStoreContext = context.GetPayload<DocumentStoreContext>();

            var userCredentials = new UserCredentials
            {
                UserName = command.UserName,
                Password = command.Password
            };

            using var session = await documentStoreContext.OpenSession(string.Empty).ConfigureAwait(false);

            session.Insert(userCredentials);

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var _ = new Identity<UserCredentials, int>(userCredentials.Id);

            _ = context.AddOrUpdatePayload(
                () => new Identity<UserCredentials, int>(userCredentials.Id),
                (_) => new Identity<UserCredentials, int>(userCredentials.Id));
        }
    }
}
