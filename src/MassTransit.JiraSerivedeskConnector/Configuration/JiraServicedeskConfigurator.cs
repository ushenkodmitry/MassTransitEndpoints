namespace MassTransit.JiraSerivedeskConnector.Configuration
{
    public sealed class JiraServicedeskConfigurator : IJiraServicedeskConfigurator
    {
        public ServerOptions Options { get; private set; }

        public void UseOptions(ServerOptions options) => Options = options;
    }
}
