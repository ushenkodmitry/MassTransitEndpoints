namespace MassTransit.Objects.Models
{
    public sealed class SmtpInstance
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SmtpConnectionId { get; set; }

        public int UserCredentialsId { get; set; }

        public int? InstancesCount { get; set; }
    }
}
