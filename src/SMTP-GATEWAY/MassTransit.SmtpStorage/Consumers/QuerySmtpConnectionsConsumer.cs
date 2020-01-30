using System;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes.Internals.Extensions;
using MassTransit.Messages;
using MassTransit.Objects.Queries;
using MassTransit.Repositories;

namespace MassTransit.Consumers
{
    public sealed class QuerySmtpConnectionsConsumer : IConsumer<QuerySmtpConnections>
    {
        readonly ISmtpConnectionsRepository _repository;

        public QuerySmtpConnectionsConsumer(ISmtpConnectionsRepository repository) => _repository = repository;

        public async Task Consume(ConsumeContext<QuerySmtpConnections> context)
        {
            var smtpConnections = await _repository.SendQuery(context, SmtpConnectionsQuery.All, context.CancellationToken).ConfigureAwait(false);

            var smtpConnectionsQueried = TypeCache<SmtpConnectionsQueried>.InitializeFromObject(new
            {
                SmtpConnections = smtpConnections.Select(e => TypeCache<SmtpConnection>.InitializeFromObject(e)).ToArray()
            });

            await context.RespondAsync(smtpConnectionsQueried).ConfigureAwait(false);
        }
    }
}
