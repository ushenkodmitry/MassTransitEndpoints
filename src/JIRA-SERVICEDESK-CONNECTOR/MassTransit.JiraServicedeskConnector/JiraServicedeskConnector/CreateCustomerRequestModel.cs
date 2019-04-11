namespace MassTransit.JiraServicedeskConnector
{
    sealed class CreateCustomerRequestModel
    {
        public string ServiceDeskId { get; set; }

        public string RequestTypeId { get; set; }

        public RequestFieldsValues RequestFieldValues { get; set; } = new RequestFieldsValues();
    }
}
