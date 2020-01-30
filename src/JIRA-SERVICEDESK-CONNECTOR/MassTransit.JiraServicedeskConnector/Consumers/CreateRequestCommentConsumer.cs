using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Context;
using MassTransit.JiraServicedeskConnector.Contexts;
using MassTransit.JiraServicedeskConnector.Contexts.Commands;
using MassTransit.JiraServicedeskConnector.Messages;

namespace MassTransit.JiraServicedeskConnector.Consumers
{
    public sealed class CreateRequestCommentConsumer : IConsumer<CreateRequestComment>
    {
        public async Task Consume(ConsumeContext<CreateRequestComment> context)
        {
            LogContext.Debug?.Log("Creating new request comment.");

            var jiraServicedeskContext = context.GetPayload<JiraServicedeskContext>();

            var createRequestCommentCommand = new CreateRequestCommentCommand
            {
                IssueIdOrKey = context.Message.IssueIdOrKey,
                Comment = context.Message.Comment,
                Public = context.Message.Public
            };

            var createRequestCommandResult = await jiraServicedeskContext.Send(createRequestCommentCommand, context.CancellationToken).ConfigureAwait(false);

            LogContext.Info?.Log($"Request comment created. Id: {createRequestCommandResult.Id}.");

            await context.Publish<RequestCommentCreated>(new
            {
                context.Message.CorrelationId,
                createRequestCommandResult.Id,
                createRequestCommandResult.CreatedAt
            }).ConfigureAwait(false);
        }
    }
}
