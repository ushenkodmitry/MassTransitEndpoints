using System;

namespace Options.MassTransit
{
    public sealed class EndpointOptions
    {
        public string QueueName { get; set; }

        public ushort PrefetchCount { get; set; } = (ushort)Environment.ProcessorCount;

        public string ExchangeType { get; set; }

        public bool Lazy { get; set; }

        public bool PurgeOnStartup { get; set; }

        public bool AutoDelete { get; set; }

        public bool Durable { get; set; } = true;

        public string RoutingKey { get; set; }
    }
}
