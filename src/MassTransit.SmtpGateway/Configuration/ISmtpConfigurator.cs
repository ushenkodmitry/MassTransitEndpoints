using MassTransit.SmtpGateway.Options;

namespace MassTransit.SmtpGateway.Configuration
{
    public interface ISmtpConfigurator
    {
        void UseOptions(ServerOptions options);

        void UseOptions(BehaviorOptions options);
    }
}
