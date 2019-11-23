using System.Threading.Tasks;
using GreenPipes.Internals.Extensions;
using MassTransit.Messages;
using MassTransit.Objects.Commands;
using MassTransit.Objects.Models;
using MassTransit.Payloads;
using MassTransit.Repositories;

namespace MassTransit.Consumers
{
    public sealed class CreateSmtpConnectionConsumer : IConsumer<CreateSmtpConnection>
    {
        readonly ISmtpConnectionsRepository _repository;

        public CreateSmtpConnectionConsumer(ISmtpConnectionsRepository repository) => _repository = repository;

        public async Task Consume(ConsumeContext<CreateSmtpConnection> context)
        {
            var createSmtpConnectionCommand = TypeCache<CreateSmtpConnectionCommand>.InitializeFromObject(context.Message);

            var createdId = context.AddCreatedId<SmtpConnection, int>();

            await _repository.SendCommand(context, createSmtpConnectionCommand, context.CancellationToken).ConfigureAwait(false);

            var smtpConnectionCreated = TypeCache<SmtpConnectionCreated>.InitializeFromObject(createdId);

            await context.Publish(smtpConnectionCreated).ConfigureAwait(false);
        }
    }
}
