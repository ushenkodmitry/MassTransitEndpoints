using Marten;
using MassTransit.Objects.Models;

namespace MassTransit.Registries
{
    sealed class UserCredentialsRegistry : MartenRegistry
    {
        public UserCredentialsRegistry()
        {
            var mapping = For<UserCredentials>();

            mapping
                .Identity(e => e.Id)
                .Duplicate(e => e.UserName)
                .DocumentAlias("usercredentials");
        }
    }
}
