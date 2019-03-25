using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Logging;
using MassTransit.SmtpGateway.Contexts;
using MassTransit.SmtpGateway.Messages;

namespace MassTransit.SmtpGateway.Pipeline.Filters
{
    public sealed class SmtpGatewayFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        static readonly ILog _log = Logger.Get<SmtpGatewayFilter<TContext>>();

        [DebuggerNonUserCode]
        public Task Send(TContext context, IPipe<TContext> next)
        {
            _log.Debug(() => "Sending through filter.");

            var smtpGatewayConnector = new SmtpGatewayConnector(context, context.CorrelationId, context.CancellationToken);

            SmtpGatewayContext smtpGatewayContext = new ConsumeSmtpGatewayContext(smtpGatewayConnector);

            context.GetOrAddPayload(() => smtpGatewayContext);

            return next.Send(context);
        }

        public void Probe(ProbeContext context) => _ = context.CreateFilterScope(nameof(SmtpGatewayFilter<TContext>));

        sealed class ConsumeSmtpGatewayContext : SmtpGatewayContext
        {
            readonly ISmtpGatewayConnector _connector;

            public ConsumeSmtpGatewayContext(ISmtpGatewayConnector connector) => _connector = connector;

            public Task SendMail(Action<ISendBuilder> build, CancellationToken cancellationToken) 
                => _connector.SendMail(build, cancellationToken);

            public Task SendMail(Action<ISendBuilder> build, IPipe<PublishContext<SendMail>> pipe, CancellationToken cancellationToken = default) 
                => _connector.SendMail(build, pipe, cancellationToken);
        }
    }
}
