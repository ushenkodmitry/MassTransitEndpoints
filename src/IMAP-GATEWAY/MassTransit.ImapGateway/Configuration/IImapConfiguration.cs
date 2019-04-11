using MassTransit.ImapGateway.Options;
using System;

namespace MassTransit.ImapGateway.Configuration
{
    public interface IImapConfigurator
    {
        void UseOptions(ServerOptions options);

        void UseOptions(BehaviorOptions options);

        void UseOptions(Action<ServerOptions> buildOptions);

        void UseOptions(Action<BehaviorOptions> buildOptions);
    }
}
