namespace MassTransit.JiraServicedeskConnector
{
    sealed class CreatedRequestCommentModel
    {
        public string Id { get; set; }

        public CreatedDate Created { get; set; }

        [Prefix("_")]
        public IssueLinks Links { get; set; }

        public JiraUser Author { get; set; }
    }
}
