using MassTransit.Options;
using System;

namespace MassTransit.Configuration
{
    public interface ISmtpStorageConfigurator
    {
        void Configure(Action<ConnectionStringsOptions> configureConnectionStrings);
    }
}
