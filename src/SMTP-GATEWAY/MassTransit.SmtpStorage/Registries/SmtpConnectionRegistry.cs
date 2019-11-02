using Marten;
using MassTransit.Objects.Models;

namespace MassTransit.Registries
{
    sealed class SmtpConnectionRegistry : MartenRegistry
    {
        public SmtpConnectionRegistry()
        {
            var mapping = For<SmtpConnection>();

            mapping
                .Identity(e => e.Id)
                .Duplicate(e => e.Name)
                .DocumentAlias("smtpconnections");
        }
    }
}
