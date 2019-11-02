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
    public sealed class CreateSmtpConnectionConsumer : IConsumer<CreateSmtpConnection>
    {
        readonly ISmtpConnectionsRepository _repository;

        public CreateSmtpConnectionConsumer(ISmtpConnectionsRepository repository) => _repository = repository;

        public async Task Consume(ConsumeContext<CreateSmtpConnection> context)
        {
            var createSmtpServerCommand = TypeCache<CreateSmtpConnectionCommand>.InitializeFromObject(context.Message);

            await _repository.SendCommand(context, createSmtpServerCommand, context.CancellationToken).ConfigureAwait(false);

            var identity = context.GetPayload<Identity<SmtpConnection, int>>();

            var smtpServerCreated = TypeCache<SmtpConnectionCreated>.InitializeFromObject(identity);

            await context.Publish(smtpServerCreated).ConfigureAwait(false);
        }
    }
}
