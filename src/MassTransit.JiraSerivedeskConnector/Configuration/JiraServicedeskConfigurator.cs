using System;

namespace MassTransit.JiraSerivedeskConnector.Configuration
{
    public sealed class JiraServicedeskConfigurator : IJiraServicedeskConfigurator
    {
        public ServerOptions ServerOptions { get; private set; }

        public OAuthOptions OAuthOptions { get; private set; }

        public BasicAuthOptions BasicAuthOptions { get; private set; }

        public void UseOptions(ServerOptions options) => ServerOptions = options;

        public void UseOptions(BasicAuthOptions options) => BasicAuthOptions = options;

        public void UseOptions(OAuthOptions options) => OAuthOptions = options;

        public void UseOptions(Action<ServerOptions> buildOptions)
        {
            ServerOptions options = new ServerOptions();
            buildOptions(options);

            UseOptions(options);
        }

        public void UseOptions(Action<BasicAuthOptions> buildOptions)
        {
            BasicAuthOptions options = new BasicAuthOptions();
            buildOptions(options);

            UseOptions(options);
        }

        public void UseOptions(Action<OAuthOptions> buildOptions)
        {
            OAuthOptions options = new OAuthOptions();
            buildOptions(options);

            UseOptions(options);
        }
    }
}
