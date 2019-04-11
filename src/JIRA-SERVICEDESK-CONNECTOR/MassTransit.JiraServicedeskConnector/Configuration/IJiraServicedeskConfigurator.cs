using MassTransit.JiraServicedeskConnector.Options;
using System;

namespace MassTransit.JiraServicedeskConnector.Configuration
{
    public interface IJiraServicedeskConfigurator
    {
        void UseOptions(ServerOptions options);

        void UseOptions(BehaviorOptions options);

        void UseOptions(BasicAuthOptions options);

        void UseOptions(OAuthOptions options);

        void UseOptions(Action<ServerOptions> buildOptions);

        void UseOptions(Action<BehaviorOptions> buildOptions);

        void UseOptions(Action<BasicAuthOptions> buildOptions);

        void UseOptions(Action<OAuthOptions> buildOptions);
    }
}
