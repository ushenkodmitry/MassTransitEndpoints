using GreenPipes;
using MailKit.Net.Imap;
using MassTransit.Context;
using MassTransit.ImapGateway.Contexts;
using MassTransit.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.ImapGateway.Pipeline.Filters
{
    public sealed class ImapFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        void IProbeSite.Probe(ProbeContext context) => context.CreateFilterScope(nameof(ImapFilter<TContext>));

        [DebuggerNonUserCode]
        async Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            LogContext.Debug?.Log("Sending through filter.");

            OptionsContext optionsContext = context.GetPayload<OptionsContext>();

            using var imapClient = new ImapClient();

            var authenticationCompleted =
                imapClient.ConnectAsync(optionsContext.ServerOptions.Host, optionsContext.ServerOptions.Port, optionsContext.ServerOptions.UseSsl)
                .ContinueWith(_ => imapClient.AuthenticateAsync(optionsContext.ServerOptions.Username, optionsContext.ServerOptions.Password))
                .ContinueWith(async _ =>
                {
                    if (optionsContext.ServerOptions.UseCompression)
                        await imapClient.CompressAsync().ConfigureAwait(false);

                    if (optionsContext.ServerOptions.UseUtf8)
                        await imapClient.EnableUTF8Async().ConfigureAwait(false);
                })
                .Unwrap();

            ImapContext imapContext = new ConsumeImapContext(context, imapClient, authenticationCompleted);

            context.GetOrAddPayload(() => imapContext);

            await next.Send(context).ConfigureAwait(false);

            await imapClient.DisconnectAsync(true, context.CancellationToken).ConfigureAwait(false);
        }

        sealed class ConsumeImapContext : ImapContext
        {
            readonly ConsumeContext _context;

            readonly ImapClient _imapClient;

            public Task AuthenticationComplated { get; }

            public ConsumeImapContext(ConsumeContext context, ImapClient imapClient, Task authenticationComplated)
            {
                _context = context;
                _imapClient = imapClient;
                AuthenticationComplated = authenticationComplated;
            }

            public async Task Noop(CancellationToken cancellationToken)
            {
                await AuthenticationComplated.ConfigureAwait(false);

                using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(_context.CancellationToken, cancellationToken);

                await _imapClient.NoOpAsync(cts.Token).ConfigureAwait(false);
            }
        }
    }
}
