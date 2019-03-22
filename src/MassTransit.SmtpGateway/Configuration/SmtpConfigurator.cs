using System;
using MassTransit.SmtpGateway.Options;

namespace MassTransit.SmtpGateway.Configuration
{
    sealed class SmtpConfigurator : ISmtpConfigurator
    {
        public ServerOptions ServerOptions { get; private set; }

        public BehaviorOptions BehaviorOptions { get; private set; }

        public void UseOptions(ServerOptions options) => ServerOptions = options;

        public void UseOptions(BehaviorOptions options) => BehaviorOptions = options;

        public void UseOptions(Action<ServerOptions> buildOptions)
        {
            ServerOptions options = new ServerOptions();
            buildOptions(options);

            UseOptions(options);
        }

        public void UseOptions(Action<BehaviorOptions> buildOptions)
        {
            BehaviorOptions options = new BehaviorOptions();
            buildOptions(options);

            UseOptions(options);
        }
    }
}
