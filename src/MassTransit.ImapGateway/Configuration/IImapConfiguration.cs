using MassTransit.ImapGateway.Options;

namespace MassTransit.ImapGateway.Configuration
{
    public interface IImapConfigurator
    {
        void UseOptions(ServerOptions options);

        void UseOptions(BehaviorOptions options);
    }
}
