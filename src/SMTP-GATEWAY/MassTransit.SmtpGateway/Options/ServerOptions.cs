using System;

namespace MassTransit.SmtpGateway.Options
{
    public sealed class ServerOptions
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public bool UseSsl { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public TimeSpan? NoOp { get; set; }
    }
}
