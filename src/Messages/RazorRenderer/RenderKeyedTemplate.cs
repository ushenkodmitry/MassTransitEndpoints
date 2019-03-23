using System;
using MassTransit;

namespace MassTransit.RazorRenderer.Messages
{
    public interface RenderKeyedTemplate : RoutedBy<string>, CorrelatedBy<Guid>
    {
        string Model { get; }

        string TemplateKey { get; }
    }
}