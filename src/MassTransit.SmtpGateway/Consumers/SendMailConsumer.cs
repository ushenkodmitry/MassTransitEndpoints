using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Logging;
using MassTransit.SmtpGateway.Contexts;
using MassTransit.SmtpGateway.Messages;
using MimeKit;
using MimeKit.Text;

namespace MassTransit.SmtpGateway.Consumers
{
    public sealed class SendMailConsumer : IConsumer<SendMail>
    {
        static readonly ILog _log = Logger.Get(nameof(SendMailConsumer));

        public async Task Consume(ConsumeContext<SendMail> context)
        {
            var smtpContext = context.GetPayload<SmtpContext>();

            var message = CreateMimeMessage(context);

            await smtpContext.Send(message, context.CancellationToken).ConfigureAwait(false);

            await context.Publish<MailSent>(new { context.Message.CorrelationId }).ConfigureAwait(false);
        }

        static MimeMessage CreateMimeMessage(ConsumeContext<SendMail> context)
        {
            MimeMessage mimeMessage = new MimeMessage
            {
                Subject = context.Message.Subject,
            };
            if (!string.IsNullOrWhiteSpace(context.Message.MessageId))
                mimeMessage.MessageId = context.Message.MessageId;
            if (!string.IsNullOrWhiteSpace(context.Message.Importance))
                mimeMessage.Importance = (MessageImportance)Enum.Parse(typeof(MessageImportance), context.Message.Importance);
            if(!string.IsNullOrWhiteSpace(context.Message.Priority))
                mimeMessage.Priority = (MessagePriority)Enum.Parse(typeof(MessagePriority), context.Message.Priority);
            if(!string.IsNullOrWhiteSpace(context.Message.XPriority))
                mimeMessage.XPriority = (XMessagePriority)Enum.Parse(typeof(XMessagePriority), context.Message.XPriority);

            Array.ForEach(context.Message.From, from =>
            {
                var splitted = from.Split(new[] { ASCII.UnitSeparator }, StringSplitOptions.None);

                if (splitted.Length == 1)
                    mimeMessage.From.Add(new MailboxAddress(splitted[0]));
                else
                    mimeMessage.From.Add(new MailboxAddress(splitted[0], splitted[1]));
            });

            Array.ForEach(context.Message.To, to =>
            {
                var splitted = to.Split(new[] { ASCII.UnitSeparator }, StringSplitOptions.None);

                if (splitted.Length == 1)
                    mimeMessage.To.Add(new MailboxAddress(splitted[0]));
                else
                    mimeMessage.To.Add(new MailboxAddress(splitted[0], splitted[1]));
            });

            Array.ForEach(context.Message.Cc ?? new string[0], to =>
            {
                var splitted = to.Split(new[] { ASCII.UnitSeparator }, StringSplitOptions.None);

                if (splitted.Length == 1)
                    mimeMessage.Cc.Add(new MailboxAddress(splitted[0]));
                else
                    mimeMessage.Cc.Add(new MailboxAddress(splitted[0], splitted[1]));
            });

            Array.ForEach(context.Message.Bcc ?? new string[0], to =>
            {
                var splitted = to.Split(new[] { ASCII.UnitSeparator }, StringSplitOptions.None);

                if (splitted.Length == 1)
                    mimeMessage.Bcc.Add(new MailboxAddress(splitted[0]));
                else
                    mimeMessage.Bcc.Add(new MailboxAddress(splitted[0], splitted[1]));
            });

            TextPart textPart = null;
            if(!string.IsNullOrWhiteSpace(context.Message.HtmlBody))
                textPart = new TextPart(TextFormat.Html) { Text = context.Message.HtmlBody };
            else if(!string.IsNullOrWhiteSpace(context.Message.TextBody))
                textPart = new TextPart(TextFormat.Text) { Text = context.Message.TextBody };

            var attachmentParts =
                context.Message.AttachmentsMeta?
                    .Select(m => m.Split(new[] { char.ConvertFromUtf32(31) }, StringSplitOptions.None))
                    .Select(m =>
                    {
                        int offset = int.Parse(m[3]);
                        int length = int.Parse(m[4]);
                        byte[] buffer = new byte[length];
                        Array.Copy(context.Message.AttachmentsContent, offset, buffer, 0, length);

                        return new MimePart
                        {
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = m[0],
                            Content = new MimeContent(new MemoryStream(buffer)),
                            ContentType =
                            {
                                MediaType = m[1],
                                MediaSubtype = m[2]
                            }
                        };
                    })
                    .ToArray() ?? new MimePart[0];

            if (attachmentParts.Length > 0)
            {
                var multipart = new Multipart("mixed");
                if (textPart != null)
                    multipart.Add(textPart);

                Array.ForEach(attachmentParts, multipart.Add);

                mimeMessage.Body = multipart;
            }
            else
            {
                if (textPart != null)
                    mimeMessage.Body = textPart;
            }

            return mimeMessage;
        }
    }
}
