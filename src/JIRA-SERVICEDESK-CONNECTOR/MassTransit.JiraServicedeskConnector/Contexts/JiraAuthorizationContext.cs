using System.Threading.Tasks;

namespace MassTransit.JiraServicedeskConnector.Contexts
{
    public interface JiraAuthorizationContext
    {
        Task<string> Authorization { get; }
    }
}
