using MassTransit.RazorRenderer.Options;

namespace MassTransit.RazorRenderer.Contexts
{
    public interface OptionsContext
    {
        BehaviorOptions BehaviorOptions { get; }
    }
}
