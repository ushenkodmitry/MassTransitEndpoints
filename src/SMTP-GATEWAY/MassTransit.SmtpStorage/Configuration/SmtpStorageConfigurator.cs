using System;
using MassTransit.Options;

namespace MassTransit.Configuration
{
    sealed class SmtpStorageConfigurator : ISmtpStorageConfigurator
    {
        public ConnectionStringsOptions ConnectionStringsOptions { get; } = new ConnectionStringsOptions();

        public void Configure(Action<ConnectionStringsOptions> configureConnectionStrings) => configureConnectionStrings(ConnectionStringsOptions);
    }
}
