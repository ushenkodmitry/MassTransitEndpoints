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
    public sealed class CreateSmtpInstanceConsumer : IConsumer<CreateSmtpInstance>
    {
       readonly ISmtpInstancesRepository _repository;

        public CreateSmtpInstanceConsumer(ISmtpInstancesRepository repository) => _repository = repository;

        public async Task Consume(ConsumeContext<CreateSmtpInstance> context)
        {
            var createSmtpInstanceCommand = TypeCache<CreateSmtpInstanceCommand>.InitializeFromObject(context.Message);

            var createdId = context.AddCreatedId<SmtpInstance, int>();

            await _repository.SendCommand(context, createSmtpInstanceCommand, context.CancellationToken).ConfigureAwait(false);

            var smtpInstanceCreated = TypeCache<SmtpInstanceCreated>.InitializeFromObject(createdId);

            await context.Publish(smtpInstanceCreated).ConfigureAwait(false);
        }
    }
}
