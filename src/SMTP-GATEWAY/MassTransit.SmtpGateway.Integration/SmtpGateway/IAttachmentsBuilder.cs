using System.IO;

namespace MassTransit.SmtpGateway
{
    public interface IAttachmentsBuilder
    {
        IAttachmentsBuilder Attach(string fileName, Stream stream, string mediaType = null, string mediaSubType = null);
    }
}
