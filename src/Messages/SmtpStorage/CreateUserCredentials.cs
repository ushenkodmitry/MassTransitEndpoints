namespace MassTransit.Messages
{
    /// <summary>
    /// Message contract to create user credentials.
    /// </summary>
    public interface CreateUserCredentials
    {
        /// <summary>
        /// Gets user name.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Gets user password.
        /// </summary>
        string Password { get; }
    }
}
