using System;
using MassTransit.Options;

namespace MassTransit.RazorRenderer.Configuration.PipeConfigurators
{
    sealed class RendererConfigurator : IRendererConfigurator
    {
        public BehaviorOptions BehaviorOptions { get; private set; }
        
        public void UseOptions(BehaviorOptions options) => BehaviorOptions = options;

        public void UseOptions(Action<BehaviorOptions> buildOptions)
        {
            BehaviorOptions options = new BehaviorOptions();
            buildOptions(options);
            
            UseOptions(options);
        }
    }
}