namespace MassTransit.JiraServicedeskConnector.Contexts.Results
{
    public sealed class CreateCustomerRequestResult
    {
        public string IssueId { get; set; }

        public string IssueKey { get; set; }

        public string WebLink { get; set; }
    }
}
