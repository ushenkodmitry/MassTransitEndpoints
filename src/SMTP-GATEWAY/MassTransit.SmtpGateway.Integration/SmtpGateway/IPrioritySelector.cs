namespace MassTransit.SmtpGateway
{
    public interface IPrioritySelector
    {
        ISendBuilder NonUrgent();

        ISendBuilder Urgent();

        ISendBuilder Normal();
    }
}
