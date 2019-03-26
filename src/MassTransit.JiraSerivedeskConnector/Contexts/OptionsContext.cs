namespace MassTransit.JiraSerivedeskConnector.Contexts
{
    public interface OptionsContext
    {
        ServerOptions ServerOptions { get; }

        OAuthOptions OAuthOptions { get; }

        BasicAuthOptions BasicAuthOptions { get; }
    }
}
