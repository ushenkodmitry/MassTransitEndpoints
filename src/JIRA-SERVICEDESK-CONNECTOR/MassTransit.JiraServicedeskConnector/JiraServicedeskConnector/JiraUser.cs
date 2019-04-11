namespace MassTransit.JiraServicedeskConnector
{
    sealed class JiraUser
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public string DisplayName { get; set; }

        public string TimeZone { get; set; }
    }
}
