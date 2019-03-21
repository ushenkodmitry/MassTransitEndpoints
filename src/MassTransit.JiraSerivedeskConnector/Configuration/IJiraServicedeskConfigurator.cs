namespace MassTransit.JiraSerivedeskConnector.Configuration
{
    public interface IJiraServicedeskConfigurator
    {
        void UseOptions(ServerOptions options);
    }
}
