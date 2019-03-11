using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MailKit.Net.Smtp;
using MassTransit.Logging;
using MassTransit.SmtpGateway.Configuration;
using MassTransit.SmtpGateway.Contexts;
using MassTransit.SmtpGateway.Options;
using MimeKit;

namespace MassTransit.SmtpGateway.Pipeline.Filters
{
    public sealed class SmtpFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<SmtpFilter<TContext>>();

        readonly ServerOptions _serverOptions;

        readonly Task _authenticationCompleted;

        readonly SmtpClient _smtpClient;

        Timer _noOpTimer;

        public SmtpFilter(Action<ISmtpConfigurator> configureSmtp)
        {
            var configurator = new SmtpConfigurator();
            configureSmtp(configurator);
            _serverOptions = configurator.Options;

            var smtpClient = new SmtpClient();
            _authenticationCompleted = smtpClient
                .ConnectAsync(configurator.Options.Host, configurator.Options.Port)
                .ContinueWith(_ => _smtpClient.AuthenticateAsync(configurator.Options.Username, configurator.Options.Password))
                .ContinueWith(_ =>
                {
                    if (_serverOptions.NoOp.HasValue)
                        _noOpTimer = new Timer(state => _smtpClient.NoOp(), null, 0, (uint)_serverOptions.NoOp.Value.TotalMilliseconds);

                    return Task.CompletedTask;
                }).Unwrap();
            _smtpClient = smtpClient;
        }

        public Task Send(TContext context, IPipe<TContext> next)
        {
            SmtpContext smtpContext = new ConsumeSmtpContext(context, _smtpClient, _authenticationCompleted);

            context.GetOrAddPayload(() => smtpContext);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(SmtpFilter<TContext>));
            scope.Set(_serverOptions);
        }

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
        }
    }
}
