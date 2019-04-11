using MassTransit.JiraServicedeskConnector.Options;

namespace MassTransit.JiraServicedeskConnector.Contexts
{
    public interface OptionsContext
    {
        ServerOptions ServerOptions { get; }

        OAuthOptions OAuthOptions { get; }

        BasicAuthOptions BasicAuthOptions { get; }
    }
}
