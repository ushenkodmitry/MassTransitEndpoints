using GreenPipes;
using MassTransit.SmtpGateway.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.SmtpGateway
{
    public interface ISmtpGatewayConnector
    {
        Task SendMail(Action<ISendBuilder> build, CancellationToken cancellationToken = default);

        Task SendMail(Action<ISendBuilder> build, IPipe<PublishContext<SendMail>> pipe, CancellationToken cancellationToken = default);
    }
}
