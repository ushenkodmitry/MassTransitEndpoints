namespace MassTransit.JiraServicedeskConnector
{
    sealed class CustomerRequestsModel
    {
        public int Size { get; set; }

        public bool IsLastPage { get; set; }

        [Prefix("_")]
        public PaginationLinks Links { get; set; }

        public CustomerRequestModel[] Values { get; set; }
    }
}
