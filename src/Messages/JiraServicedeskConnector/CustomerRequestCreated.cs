using System;

namespace MassTransit.JiraServicedeskConnector.Messages
{
    public interface CustomerRequestCreated : CorrelatedBy<Guid>
    {
        string IssueId { get; }

        string IssueKey { get; }

        string WebLink { get; }

        DateTimeOffset CreatedAt { get; }
    }
}
