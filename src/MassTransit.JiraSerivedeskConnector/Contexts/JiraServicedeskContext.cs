using System.Threading.Tasks;

namespace MassTransit.JiraSerivedeskConnector.Contexts
{
    public interface JiraServicedeskContext
    {
        Task AuthenticationCompleted { get; }
    }
}
