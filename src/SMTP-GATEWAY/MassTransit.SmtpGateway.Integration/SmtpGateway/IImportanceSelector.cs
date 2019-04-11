namespace MassTransit.SmtpGateway
{
    public interface IImportanceSelector
    {
        ISendBuilder Low();

        ISendBuilder Normal();

        ISendBuilder High();
    }
}
