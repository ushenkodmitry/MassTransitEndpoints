namespace MassTransit.JiraServicedeskConnector
{
    sealed class PaginationLinks
    {
        public string Prev { get; set; }

        public string Next { get; set; }
    }
}
