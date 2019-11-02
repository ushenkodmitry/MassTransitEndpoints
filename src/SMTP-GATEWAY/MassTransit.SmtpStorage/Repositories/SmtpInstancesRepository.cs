﻿using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Contexts;
using MassTransit.Objects.Commands;
using MassTransit.Objects.Models;
using MassTransit.Payloads;

namespace MassTransit.Repositories
{
    sealed class SmtpInstancesRepository : ISmtpInstancesRepository
    {
        public async Task SendCommand(PipeContext context, CreateSmtpInstanceCommand command, CancellationToken cancellationToken = default)
        {
            var documentStoreContext = context.GetPayload<DocumentStoreContext>();

            var smtpInstance = new SmtpInstance
            {
                InstancesCount = command.InstancesCount,
                SmtpConnectionId = command.SmtpConnectionId,
                UserCredentialsId = command.UserCredentialsId,
                Name = command.Name
            };

            using var session = await documentStoreContext.OpenSession(string.Empty).ConfigureAwait(false);

            session.Insert(smtpInstance);

            await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _ = context.AddOrUpdatePayload(
                () => new Identity<SmtpInstance, int>(smtpInstance.Id),
                (_) => new Identity<SmtpInstance, int>(smtpInstance.Id));
        }
    }
}
