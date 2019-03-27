namespace MassTransit.JiraServicedeskConnector
{
    sealed class ServicedesksModel
    {
        [Prefix("_")]
        public PaginationLinks Links { get; set; }

        public ServicedeskModel[] Values { get; set; }
    }
}
