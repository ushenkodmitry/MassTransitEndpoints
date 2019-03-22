using GreenPipes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.SmtpGateway.Contexts
{
    public static class ContextsExtensions
    {
        public static Task SendMail(this ConsumeContext context, Action<ISendBuilder> builder, CancellationToken cancellationToken = default) => context.GetPayload<SmtpGatewayContext>().SendMail(builder, cancellationToken);
    }
}
