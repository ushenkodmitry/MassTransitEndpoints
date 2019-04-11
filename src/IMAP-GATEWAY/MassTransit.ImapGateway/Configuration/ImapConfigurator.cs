using System;
using MassTransit.ImapGateway.Options;

namespace MassTransit.ImapGateway.Configuration
{
    sealed class ImapConfigurator : IImapConfigurator
    {
        public ServerOptions ServerOptions { get; private set; }

        public BehaviorOptions BehaviorOptions { get; private set; }

        public void UseOptions(BehaviorOptions options) => BehaviorOptions = options;

        public void UseOptions(ServerOptions options) => ServerOptions = options;

        public void UseOptions(Action<ServerOptions> buildOptions)
        {
            var options = new ServerOptions();
            buildOptions(options);

            UseOptions(options);
        }

        public void UseOptions(Action<BehaviorOptions> buildOptions)
        {
            var options = new BehaviorOptions();
            buildOptions(options);

            UseOptions(options);
        }
    }
}
