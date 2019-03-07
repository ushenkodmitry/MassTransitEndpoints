namespace Builders
{
    public interface IMailboxesBuilder
    {
        IMailboxesBuilder Mailbox(string name, string address);
    }
}
