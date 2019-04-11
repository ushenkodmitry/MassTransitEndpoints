namespace MassTransit.JiraServicedeskConnector.Contexts.Commands
{
    public sealed class CreateCustomerRequestCommand
    {
        public string ServiceDeskId { get; set; }

        public string RequestTypeId { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }
    }
}
