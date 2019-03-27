using System;

namespace MassTransit.JiraServicedeskConnector
{
    sealed class CreatedDate
    {
        public DateTimeOffset Iso8601 { get; set; }

        public string Friendly { get; set; }
    }
}
