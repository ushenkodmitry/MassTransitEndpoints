using MassTransit.JiraServicedeskConnector.Contexts.Commands;
using MassTransit.JiraServicedeskConnector.Contexts.Queries;
using MassTransit.JiraServicedeskConnector.Contexts.Results;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.JiraServicedeskConnector.Contexts
{
    public interface JiraServicedeskContext
    {
        Task<CreateCustomerRequestResult> Send(CreateCustomerRequestCommand command, CancellationToken cancellationToken = default);

        Task Send(CustomerRequestQuery query, CancellationToken cancellationToken = default);

        Task Send(MyCustomerRequestsQuery query, CancellationToken cancellationToken = default);

        Task<CreateRequestCommentResult> Send(CreateRequestCommentCommand command, CancellationToken cancellationToken = default);
    }
}
