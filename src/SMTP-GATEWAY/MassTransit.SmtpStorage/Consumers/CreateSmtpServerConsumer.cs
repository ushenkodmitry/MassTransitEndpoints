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
    public sealed class CreateSmtpServerConsumer : IConsumer<CreateSmtpServer>
    {
        readonly ISmtpServersRepository _repository;

        public CreateSmtpServerConsumer(ISmtpServersRepository repository) => _repository = repository;

        public async Task Consume(ConsumeContext<CreateSmtpServer> context)
        {
            var createSmtpServerCommand = TypeCache<CreateSmtpServerCommand>.InitializeFromObject(context.Message);

            await _repository.SendCommand(context, createSmtpServerCommand, context.CancellationToken).ConfigureAwait(false);

            var identity = context.GetPayload<Identity<SmtpServer, int>>();

            var smtpServerCreated = TypeCache<SmtpServerCreated>.InitializeFromObject(identity);

            await context.Publish(smtpServerCreated).ConfigureAwait(false);
        }
    }
}
