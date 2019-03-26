using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.JiraSerivedeskConnector.Contexts
{
    public interface JiraAuthorizationContext
    {
        Task<string> Authorization { get; }
    }
}
