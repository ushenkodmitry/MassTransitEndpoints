using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.JiraServicedeskConnector
{
    sealed class ServicedeskModel
    {
        public string Id { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectKey { get; set; }

        [Prefix("_")]
        public IssueLinks Links { get; set; }
    }
}
