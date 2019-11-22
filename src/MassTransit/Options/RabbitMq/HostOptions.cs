namespace Options.RabbitMq
{
    public sealed class HostOptions
    {
        public string Host { get; set; } = "localhost";

        public string VirtualHost { get; set; }

        public string Username { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public string[] ClusterMembers { get; set; } = new string[0];

        public ushort Heartbeat { get; set; }
    }
}
