namespace MassTransit.Messages
{
    /// <summary>
    /// Event about successful smtp server creation.
    /// </summary>
    public interface SmtpServerCreated
    {
        /// <summary>
        /// Gets an identity of the created smtp server.
        /// </summary>
        int Id { get; }
    }
}
