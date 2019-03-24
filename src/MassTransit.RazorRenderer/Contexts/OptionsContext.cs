using MassTransit.Options;

namespace MassTransit.RazorRenderer.Contexts
{
    public interface OptionsContext
    {
        BehaviorOptions BehaviorOptions { get; }
    }
}
