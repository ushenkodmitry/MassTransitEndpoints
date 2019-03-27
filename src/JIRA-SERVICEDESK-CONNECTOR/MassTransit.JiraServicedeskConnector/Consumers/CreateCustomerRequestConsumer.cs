using GreenPipes;
using MassTransit.JiraServicedeskConnector.Contexts;
using MassTransit.JiraServicedeskConnector.Contexts.Commands;
using MassTransit.JiraServicedeskConnector.Messages;
using MassTransit.Logging;
using System.Threading.Tasks;

namespace MassTransit.JiraServicedeskConnector.Consumers
{
    public sealed class CreateCustomerRequestConsumer : IConsumer<CreateCustomerRequest>
    {
        static readonly ILog _log = Logger.Get(nameof(CreateCustomerRequestConsumer));

        public async Task Consume(ConsumeContext<CreateCustomerRequest> context)
        {
            _log.Debug(() => "Creating new customer request.");

            var jiraServicedeskContext = context.GetPayload<JiraServicedeskContext>();

            CreateCustomerRequestCommand createCustomerRequestCommand = new CreateCustomerRequestCommand
            {
                Description = context.Message.Description,
                RequestTypeId = context.Message.RequestTypeId,
                ServiceDeskId = context.Message.ServiceDeskId,
                Summary = context.Message.Summary
            };

            var createCustomerRequestResult = await jiraServicedeskContext.Send(createCustomerRequestCommand, context.CancellationToken).ConfigureAwait(false);

            _log.Info(() => $"Customer request {createCustomerRequestResult.IssueKey} created.");

            await context.Publish<CustomerRequestCreated>(new
            {
                context.Message.CorrelationId,
                createCustomerRequestResult.IssueId,
                createCustomerRequestResult.IssueKey,
                createCustomerRequestResult.WebLink
            }).ConfigureAwait(false);
        }
    }
}
