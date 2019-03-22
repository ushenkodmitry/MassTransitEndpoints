using MassTransit.SmtpGateway.Options;
using System;

namespace MassTransit.SmtpGateway.Configuration
{
    public interface ISmtpConfigurator
    {
        void UseOptions(ServerOptions options);

        void UseOptions(BehaviorOptions options);

        void UseOptions(Action<ServerOptions> buildOptions);

        void UseOptions(Action<BehaviorOptions> buildOptions);
    }
}
