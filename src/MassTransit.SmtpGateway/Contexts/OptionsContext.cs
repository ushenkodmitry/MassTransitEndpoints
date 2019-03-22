using MassTransit.SmtpGateway.Options;

namespace MassTransit.SmtpGateway.Contexts
{
    public interface OptionsContext
    {
        ServerOptions ServerOptions { get; }
    }
}
