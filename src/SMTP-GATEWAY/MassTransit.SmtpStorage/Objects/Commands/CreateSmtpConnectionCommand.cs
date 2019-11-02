namespace MassTransit.Objects.Commands
{
    /// <summary>
    /// Repository command to create smtp connection.
    /// </summary>
    public sealed class CreateSmtpConnectionCommand
    {
        /// <summary>
        /// Gets or sets a server's friendly name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a server host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value, indicating thar secure connection must be used.
        /// </summary>
        public bool UseSsl { get; set; }
    }
}
