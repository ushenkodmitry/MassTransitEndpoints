namespace MassTransit.Objects.Commands
{
    /// <summary>
    /// Repository command to create instance by connect smtpserver and usercredentials.
    /// </summary>
    public sealed class CreateSmtpInstanceCommand
    {
        /// <summary>
        /// Gets or sets instance friendly name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets id of smtp connection to use.
        /// </summary>
        public int SmtpConnectionId { get; set; }

        /// <summary>
        /// Gets or sets id of the user credentials to use.
        /// </summary>
        public int UserCredentialsId { get; set; }

        /// <summary>
        /// Gets or sets how many instances of the connection will be available for each smtpgateway service instance.
        /// </summary>
        public int? InstancesCount { get; set; }
    }
}
