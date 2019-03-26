using GreenPipes;
using MassTransit.JiraSerivedeskConnector.Contexts;
using MassTransit.Logging;
using System.Threading.Tasks;

namespace MassTransit.JiraSerivedeskConnector.Pipeline.Filters
{
    public sealed class JiraOAuthFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<JiraOAuthFilter<TContext>>();

        readonly OAuthOptions _options;

        public JiraOAuthFilter(OAuthOptions options) => _options = options;

        public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(JiraOAuthFilter<TContext>)).Set(_options);

        public Task Send(TContext context, IPipe<TContext> next)
        {
            _log.Debug(() => "Sending through filter.");

            OptionsContext optionsContext = context.GetPayload<OptionsContext>();

            JiraAuthorizationContext jiraAuthorizationContext = new ConsumeJiraAuthorizationContext(optionsContext.ServerOptions, _options);

            context.GetOrAddPayload(() => jiraAuthorizationContext);

            return next.Send(context);
        }

        sealed class ConsumeJiraAuthorizationContext : JiraAuthorizationContext
        {
            readonly ServerOptions _serverOptions;

            readonly OAuthOptions _options;

            public Task<string> Authorization { get; private set; }

            public ConsumeJiraAuthorizationContext(ServerOptions serverOptions, OAuthOptions options)
            {
                _serverOptions = serverOptions;
                _options = options;
            }
        }
    }
}
