using System;

namespace MassTransit.RazorRenderer.Messages
{
    public interface TemplateRendered : CorrelatedBy<Guid>
    {
        string Output { get; }
    }
}