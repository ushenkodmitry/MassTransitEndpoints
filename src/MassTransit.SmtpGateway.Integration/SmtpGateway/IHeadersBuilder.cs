using System.Text;

namespace MassTransit.SmtpGateway
{
    public interface IHeadersBuilder
    {
        IHeadersBuilder Header(string field, string value, Encoding encoding = null);
    }
}
