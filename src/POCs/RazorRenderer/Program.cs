using GreenPipes.Internals.Extensions;
using MassTransit;
using MassTransit.RazorRenderer.Messages;
using System;
using System.Threading.Tasks;
using MassTransit.RazorRenderer.Configuration;
using MassTransit.RazorRenderer.Options;

namespace RazorRenderer
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(inMemory =>
            {
                inMemory.ReceiveEndpoint(endpoint =>
                {
                    endpoint.UseRazorRenderer(renderer =>
                    {
                        renderer.UseOptions((BehaviorOptions options) =>
                        {
                            options.TemplatesFolder = "Templates";
                        });
                    });
                });
            });

            await bus.StartAsync();

            await RenderKeyedTemplate(bus);

            await bus.StopAsync();

            await Console.In.ReadLineAsync();
        }

        async static Task RenderKeyedTemplate(IBus bus)
        {
            RenderKeyedTemplate renderKeyedTemplate = TypeCache<RenderKeyedTemplate>.InitializeFromObject(new
            {
                TemplateKey = "Template.cshtml",
                Model = "{ \"Who\": \"everybody\"}"
            });

            await bus.Publish(renderKeyedTemplate);
        }
    }
}
