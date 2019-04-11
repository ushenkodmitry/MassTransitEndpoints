using System;

namespace MassTransit.SmtpGateway.Options
{
    public sealed class BehaviorOptions
    { 
        public TimeSpan NoopInterval { get; set; }
    }
}
