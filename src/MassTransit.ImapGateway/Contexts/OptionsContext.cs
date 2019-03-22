using MassTransit.ImapGateway.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.ImapGateway.Contexts
{
    public interface OptionsContext
    {
        ServerOptions ServerOptions { get; }

        BehaviorOptions BehaviorOptions { get; }
    }
}
