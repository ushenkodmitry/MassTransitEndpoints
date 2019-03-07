using MassTransit.SmtpGateway.Options;

namespace MassTransit.SmtpGateway.Configuration
{
    sealed class SmtpConfigurator : ISmtpConfigurator
    {
        public ServerOptions Options { get; private set; }

        public void UseOptions(ServerOptions options) => Options = options;
    }
}
