using System.Text;

namespace Builders
{
    public interface IHeadersBuilder
    {
        IHeadersBuilder Header(string field, string value, Encoding encoding = null);
    }
}
