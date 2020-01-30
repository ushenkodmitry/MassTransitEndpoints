using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.Objects.Queries
{
    public sealed class SmtpConnectionsQuery
    {
        public static readonly SmtpConnectionsQuery All = new SmtpConnectionsQuery();
    }
}
