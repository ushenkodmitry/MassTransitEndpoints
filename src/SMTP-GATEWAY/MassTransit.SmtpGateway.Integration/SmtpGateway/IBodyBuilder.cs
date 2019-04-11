using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.SmtpGateway
{
    public interface IBodyBuilder
    {
        IBodyBuilder TextBody(string body);

        IBodyBuilder HtmlBody(string body);
    }
}
