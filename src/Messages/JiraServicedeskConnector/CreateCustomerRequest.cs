using System;

namespace MassTransit.JiraServicedeskConnector.Messages
{
    public interface CreateCustomerRequest : RoutedBy<string>, CorrelatedBy<Guid>
    {
        string ServiceDeskId { get; }

        string RequestTypeId { get; }

        string Summary { get; }

        string Description { get; }
    }
}
