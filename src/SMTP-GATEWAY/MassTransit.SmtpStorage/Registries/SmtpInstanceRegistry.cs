using Marten;
using MassTransit.Objects.Models;

namespace MassTransit.Registries
{
    sealed class SmtpInstanceRegistry : MartenRegistry
    {
        public SmtpInstanceRegistry()
        {
            var mapping = For<SmtpInstance>();

            mapping
                .Identity(e => e.Id)
                .Duplicate(e => e.Name)
                .DocumentAlias("smtpinstances");
        }
    }
}
