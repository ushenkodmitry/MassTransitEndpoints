namespace MassTransit.Messages
{
    /// <summary>
    /// Message contract of smt instance creation event.
    /// </summary>
    public interface SmtpInstanceCreated
    {
        /// <summary>
        /// Gets id of created instance.
        /// </summary>
        public int Id { get; set; }
    }
}
