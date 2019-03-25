using System;

namespace MassTransit.SmtpGateway
{
    public interface ISendBuilder
    {
        IImportanceSelector WithImportance { get; }

        IPrioritySelector WithPriority { get; }

        IXPrioritySelector WithXPriority { get; }

        ISendBuilder WithCorrelationId(Guid correlationId);

        ISendBuilder WithSubject(string subject);

        ISendBuilder WithRoutingKey(string routingKey);

        ISendBuilder WithHeaders(Action<IHeadersBuilder> headers);

        ISendBuilder WithMessageId(string messageId);

        ISendBuilder From(Action<IMailboxesBuilder> from);

        ISendBuilder To(Action<IMailboxesBuilder> to);

        ISendBuilder Cc(Action<IMailboxesBuilder> cc);

        ISendBuilder Bcc(Action<IMailboxesBuilder> bcc);

        ISendBuilder WithAttachments(Action<IAttachmentsBuilder> attachments);

        ISendBuilder WithBody(Action<IBodyBuilder> body);
    }
}