using System.Threading.Tasks;
using GreenPipes;
using GreenPipes.Internals.Extensions;
using MassTransit.Messages;
using MassTransit.Objects.Commands;
using MassTransit.Objects.Models;
using MassTransit.Payloads;
using MassTransit.Repositories;

namespace MassTransit.Consumers
{
    public sealed class CreateUserCredentialsConsumer : IConsumer<CreateUserCredentials>
    {
        readonly IUserCredentialsRepository _repository;

        public CreateUserCredentialsConsumer(IUserCredentialsRepository repository) => _repository = repository;

        public async Task Consume(ConsumeContext<CreateUserCredentials> context)
        {
            var createUserCredentialsCommand = TypeCache<CreateUserCredentialsCommand>.InitializeFromObject(context.Message);

            await _repository.SendCommand(context, createUserCredentialsCommand, context.CancellationToken).ConfigureAwait(false);

            var identity = context.GetPayload<Identity<UserCredentials, int>>();

            var smtpServerCreated = TypeCache<SmtpServerCreated>.InitializeFromObject(identity);

            await context.Publish(smtpServerCreated).ConfigureAwait(false);
        }
    }
}
