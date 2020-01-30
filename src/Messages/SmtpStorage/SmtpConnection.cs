namespace MassTransit.Messages
{
    public interface SmtpConnection
    {
        int Id { get; }

        string Name { get; }

        string Host { get; }
    }
}
