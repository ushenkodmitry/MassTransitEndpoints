namespace MassTransit.JiraServicedeskConnector
{
    sealed class CreateRequestCommentModel
    {
        public string Comment { get; set; }

        public bool Public { get; set; }
    }
}
