namespace MassTransit.Objects.Commands
{
    public sealed class CreateUserCredentialsCommand
    {
        /// <summary>
        /// Gets or sets username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        public string Password { get; set; }
    }
}
