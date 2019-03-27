using GreenPipes;
using MassTransit.JiraServicedeskConnector.Contexts;
using MassTransit.JiraServicedeskConnector.Contexts.Commands;
using MassTransit.JiraServicedeskConnector.Messages;
using MassTransit.Logging;
using System.Threading.Tasks;

namespace MassTransit.JiraServicedeskConnector.Consumers
{
    public sealed class CreateRequestCommentConsumer : IConsumer<CreateRequestComment>
    {
        static readonly ILog _log = Logger.Get<CreateRequestCommentConsumer>();

        public async Task Consume(ConsumeContext<CreateRequestComment> context)
        {
            _log.Debug(() => "Creating new request comment.");

            var jiraServicedeskContext = context.GetPayload<JiraServicedeskContext>();

            var createRequestCommentCommand = new CreateRequestCommentCommand
            {
                IssueIdOrKey = context.Message.IssueIdOrKey,
                Comment = context.Message.Comment,
                Public = context.Message.Public
            };

            var createRequestCommandResult = await jiraServicedeskContext.Send(createRequestCommentCommand, context.CancellationToken).ConfigureAwait(false);

            _log.Info(() => $"Request comment created. Id: {createRequestCommandResult.Id}.");

            await context.Publish<RequestCommentCreated>(new
            {
                context.Message.CorrelationId,
                createRequestCommandResult.Id,
                createRequestCommandResult.CreatedAt
            }).ConfigureAwait(false);
        }
    }
}
