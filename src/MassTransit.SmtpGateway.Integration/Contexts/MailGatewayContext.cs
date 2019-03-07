using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.SmtpGateway;

namespace MassTransit.Contexts
{
    public interface MailGatewayContext
    {
        Task SendMail(Action<ISendBuilder> builder, CancellationToken cancellationToken = default);
    }
}
