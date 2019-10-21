namespace MassTransit.Objects.Models
{
    public sealed class SmtpServer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool UseSsl { get; set; }
    }
}
