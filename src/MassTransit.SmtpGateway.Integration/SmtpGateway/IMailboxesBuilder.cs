namespace MassTransit.SmtpGateway
{
    public interface IMailboxesBuilder
    {
        IMailboxesBuilder Mailbox(string name, string address);
    }
}
