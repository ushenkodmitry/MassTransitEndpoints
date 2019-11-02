namespace MassTransit.Messages
{
    /// <summary>
    /// Event about successful smtp server creation.
    /// </summary>
    public interface SmtpConnectionCreated
    {
        /// <summary>
        /// Gets an identity of the created smtp connection.
        /// </summary>
        int Id { get; }
    }
}
