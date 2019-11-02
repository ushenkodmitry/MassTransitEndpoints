namespace MassTransit.Messages
{
    public interface CreateSmtpInstance
    {
        public string Name { get; }

        int SmtpConnectionId { get; }

        int UserCredentialsId { get; }

        int? InstancesCount { get; }
    }
}
