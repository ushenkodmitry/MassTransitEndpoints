using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.JiraServicedeskConnector.Messages
{
    public interface RequestCommentCreated : CorrelatedBy<Guid>
    {
        string Id { get; }

        DateTimeOffset CreatedAt { get; }
    }
}
