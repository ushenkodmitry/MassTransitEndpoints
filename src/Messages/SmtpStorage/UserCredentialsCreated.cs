namespace MassTransit.Messages
{
    /// <summary>
    /// Message contract of user credentials creation event.
    /// </summary>
    public interface UserCredentialsCreated
    {
        /// <summary>
        /// Gets id of created user credentials.
        /// </summary>
        public int Id { get; }
    }
}
