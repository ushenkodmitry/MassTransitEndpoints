﻿using MassTransit.SmtpGateway.Options;

namespace MassTransit.SmtpGateway.Configuration
{
    sealed class SmtpConfigurator : ISmtpConfigurator
    {
        public ServerOptions ServerOptions { get; private set; }

        public BehaviorOptions BehaviorOptions { get; private set; }

        public void UseOptions(ServerOptions options) => ServerOptions = options;

        public void UseOptions(BehaviorOptions options) => BehaviorOptions = options;
    }
}
