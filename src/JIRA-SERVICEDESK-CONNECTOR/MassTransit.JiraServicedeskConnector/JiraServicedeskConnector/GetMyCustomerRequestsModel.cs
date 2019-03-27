namespace MassTransit.JiraServicedeskConnector
{
    sealed class GetMyCustomerRequestsModel
    {
        public string SearchTerm { get; set; }

        public int ServiceDeskId { get; set; }

        public int RequestTypeId { get; set; }

        public int Start { get; set; }

        public int Limit { get; set; }
    }
}
