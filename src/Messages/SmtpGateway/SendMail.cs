using System;

namespace MassTransit.SmtpGateway.Messages
{
    public interface SendMail : RoutedBy<string>, CorrelatedBy<Guid>
    {
        string Subject { get; }

        string Importance { get; }

        string Priority { get; }

        string XPriority { get; }

        string TextBody { get; }

        string HtmlBody { get; }

        string MessageId { get; }

        string[] To { get; }

        string[] Bcc { get; }

        string[] Cc { get; }

        string[] From { get; }

        string[] Headers { get; }

        string[] AttachmentsMeta { get; }

        byte[] AttachmentsContent { get; }
    }
}
