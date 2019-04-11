using System;

namespace MassTransit.JiraServicedeskConnector.Contexts.Results
{
    public sealed class CreateRequestCommentResult
    {
        public string Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
