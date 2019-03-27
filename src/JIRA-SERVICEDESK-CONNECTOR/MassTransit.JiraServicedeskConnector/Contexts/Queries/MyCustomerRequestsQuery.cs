namespace MassTransit.JiraServicedeskConnector.Contexts.Queries
{
    public sealed class MyCustomerRequestsQuery
    {
        public string SearchTerm { get; set; }

        public int Start { get; set; }

        public int Limit { get; set; }

        public int ServiceDeskId { get; set; }

        public int RequestTypeId { get; set; }
    }
}
