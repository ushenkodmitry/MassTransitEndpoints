using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.JiraServicedeskConnector
{
    sealed class RequestParticipantsModel
    {
        public int Size { get; set; }

        public bool IsLastPage { get; set; }

        [Prefix("_")]
        public PaginationLinks Links { get; set; }

        public JiraUser[] Values { get; set; }
    }
}
