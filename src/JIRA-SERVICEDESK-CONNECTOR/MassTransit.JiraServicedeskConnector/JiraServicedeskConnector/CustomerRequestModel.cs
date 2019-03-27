namespace MassTransit.JiraServicedeskConnector
{
    sealed class CustomerRequestModel
    {
        public string IssueId { get; set; }

        public string IssueKey { get; set; }

        [Prefix("_")]
        public IssueLinks Links { get; set; }

        public CreatedDate CreatedDate { get; set; }

        public JiraUser Reporter { get; set; }
    }
}
