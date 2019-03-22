using System;

namespace MassTransit.ImapGateway.Options
{
    public sealed class BehaviorOptions
    {
        public TimeSpan FetchInterval { get; set; }

        public TimeSpan NoopInterval { get; set; }
    }
}
