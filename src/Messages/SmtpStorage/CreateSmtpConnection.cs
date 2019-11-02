namespace MassTransit.Messages
{
    /// <summary>
    /// A command to create smtp connection.
    /// </summary>
    public interface CreateSmtpConnection
    {
        /// <summary>
        /// Gets a server's friendly name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a server host.
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Gets port.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets a value, indicating thar secure connection must be used.
        /// </summary>
        bool UseSsl { get; }
    }
}
