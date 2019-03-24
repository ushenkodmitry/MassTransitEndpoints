using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MailKit.Net.Smtp;
using MassTransit.Logging;
using MassTransit.SmtpGateway.Contexts;
using MimeKit;

namespace MassTransit.SmtpGateway.Pipeline.Filters
{
    public sealed class SmtpFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<SmtpFilter<TContext>>();

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            _log.Debug(() => "Sending through filter.");

            OptionsContext optionsContext = context.GetPayload<OptionsContext>();

            using (var smtpClient = new SmtpClient())
            {
                var authenticationCompleted = smtpClient
                    .ConnectAsync(optionsContext.ServerOptions.Host, optionsContext.ServerOptions.Port, optionsContext.ServerOptions.UseSsl)
                    .ContinueWith(_ => smtpClient.AuthenticateAsync(optionsContext.ServerOptions.Username, optionsContext.ServerOptions.Password))
                    .Unwrap();

                SmtpContext smtpContext = new ConsumeSmtpContext(context, smtpClient, authenticationCompleted);

                context.GetOrAddPayload(() => smtpContext);

                await next.Send(context).ConfigureAwait(false);

                await smtpClient.DisconnectAsync(true, context.CancellationToken).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(SmtpFilter<TContext>));

        sealed class ConsumeSmtpContext : SmtpContext
        {
            readonly TContext _context;

            readonly SmtpClient _smtpClient;

            public Task AuthenticationCompleted { get; }

            public ConsumeSmtpContext(TContext context, SmtpClient smtpClient, Task authenticationCompleted)
            {
                _context = context;
                _smtpClient = smtpClient;
                AuthenticationCompleted = authenticationCompleted;
            }

            public async Task Send(MimeMessage message, CancellationToken cancellationToken)
            {
                await AuthenticationCompleted.ConfigureAwait(false);

                using (var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _context.CancellationToken))
                    await _smtpClient.SendAsync(message, cancellationTokenSource.Token).ConfigureAwait(false);
            }

            public async Task Noop(CancellationToken cancellationToken)
            {
                await AuthenticationCompleted.ConfigureAwait(false);

                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(_context.CancellationToken, cancellationToken))
                    await _smtpClient.NoOpAsync(cts.Token).ConfigureAwait(false);
            }
        }
    }
}
