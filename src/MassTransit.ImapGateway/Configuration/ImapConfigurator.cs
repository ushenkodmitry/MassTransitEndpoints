using MassTransit.ImapGateway.Options;

namespace MassTransit.ImapGateway.Configuration
{
    sealed class ImapConfigurator : IImapConfigurator
    {
        public ServerOptions ServerOptions { get; private set; }

        public BehaviorOptions BehaviorOptions { get; private set; }

        public void UseOptions(BehaviorOptions options) => BehaviorOptions = options;

        public void UseOptions(ServerOptions options) => ServerOptions = options;
    }
}
