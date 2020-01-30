namespace MassTransit.Messages
{
    public interface SmtpConnectionsQueried
    {
        SmtpConnection[] SmtpConnections { get; }
    }
}
