namespace MassTransit.SmtpGateway
{
    public interface IXPrioritySelector
    {
        ISendBuilder Lowest();

        ISendBuilder Low();

        ISendBuilder Normal();

        ISendBuilder High();

        ISendBuilder Highest();
    }
}
