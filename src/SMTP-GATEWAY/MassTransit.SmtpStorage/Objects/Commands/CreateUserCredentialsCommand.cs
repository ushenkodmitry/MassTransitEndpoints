namespace MassTransit.Objects.Commands
{
    public sealed class CreateUserCredentialsCommand
    {
        /// <summary>
        /// Gets or sets a user's name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a user's password.
        /// </summary>
        public string Password { get; set; }
    }
}
