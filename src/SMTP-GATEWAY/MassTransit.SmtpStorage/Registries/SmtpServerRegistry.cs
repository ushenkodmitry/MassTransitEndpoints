using Marten;
using MassTransit.Objects.Models;

namespace MassTransit.Registries
{
    sealed class SmtpServerRegistry : MartenRegistry
    {
        public SmtpServerRegistry()
        {
            var mapping = For<SmtpServer>();

            mapping
                .Identity(e => e.Id)
                .Duplicate(e => e.Name)
                .DocumentAlias("smtpservers");
        }
    }
}
