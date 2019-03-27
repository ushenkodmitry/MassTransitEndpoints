using System;

namespace MassTransit.JiraServicedeskConnector.Messages
{
    public interface CreateRequestComment : CorrelatedBy<Guid>
    {
        string IssueIdOrKey { get; }

        string Comment { get; }

        bool Public { get; }
    }
}
