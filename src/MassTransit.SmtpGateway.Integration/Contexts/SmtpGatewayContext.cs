using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.SmtpGateway.Contexts
{
    public interface SmtpGatewayContext
    {
        Task SendMail(Action<ISendBuilder> builder, CancellationToken cancellationToken = default);
    }
}
