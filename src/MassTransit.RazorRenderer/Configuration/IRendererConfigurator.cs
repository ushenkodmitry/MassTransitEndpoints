using System;
using MassTransit.Options;

namespace MassTransit.RazorRenderer.Configuration.PipeConfigurators
{
    public interface IRendererConfigurator
    {
        void UseOptions(BehaviorOptions options);
        
        void UseOptions(Action<BehaviorOptions> buildOptions);
    }
}