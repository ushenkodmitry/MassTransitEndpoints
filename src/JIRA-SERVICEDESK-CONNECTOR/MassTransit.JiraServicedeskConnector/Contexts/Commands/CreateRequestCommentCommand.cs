namespace MassTransit.JiraServicedeskConnector.Contexts.Commands
{
    public sealed class CreateRequestCommentCommand
    {
        public string IssueIdOrKey { get; set; }

        public string Comment { get; set; }

        public bool Public { get; set; }
    }
}
