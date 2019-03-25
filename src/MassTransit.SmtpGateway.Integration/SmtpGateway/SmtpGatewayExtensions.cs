using GreenPipes;
using MassTransit.SmtpGateway.Contexts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.SmtpGateway
{
    public static class SmtpGatewayExtensions
    {
        public static Task SendMail(this ConsumeContext context, Action<ISendBuilder> builder, CancellationToken cancellationToken = default)
            => context.GetPayload<SmtpGatewayContext>().SendMail(builder, cancellationToken);

        public static Task SendMail(this ConsumeContext context, Action<ISendBuilder> builder, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            => context.GetPayload<SmtpGatewayContext>().SendMail(builder, pipe, cancellationToken);

        public static Task SendMail(this IPublishEndpoint publishEndpoint, Action<ISendBuilder> build, CancellationToken cancellationToken = default) 
            => new SmtpGatewayConnector(publishEndpoint).SendMail(build, cancellationToken);

        public static Task SendMail(this IPublishEndpoint publishEndpoint, Action<ISendBuilder> build, IPipe<SendContext> pipe, CancellationToken cancellationToken = default) 
            => new SmtpGatewayConnector(publishEndpoint).SendMail(build, pipe, cancellationToken);
    }
}
