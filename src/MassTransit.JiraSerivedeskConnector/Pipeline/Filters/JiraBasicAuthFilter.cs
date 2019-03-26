using GreenPipes;
using MassTransit.JiraSerivedeskConnector.Contexts;
using MassTransit.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.JiraSerivedeskConnector.Pipeline.Filters
{
    public sealed class JiraBasicAuthFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<JiraBasicAuthFilter<TContext>>();

        readonly BasicAuthOptions _options;

        public JiraBasicAuthFilter(BasicAuthOptions options) => _options = options;

        public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(JiraBasicAuthFilter<TContext>)).Set(_options);

        public Task Send(TContext context, IPipe<TContext> next)
        {
            JiraAuthorizationContext jiraAuthorizationContext = new ConsumeJiraAuthorizationContext(_options);

            context.GetOrAddPayload(() => jiraAuthorizationContext);

            return next.Send(context);
        }

        sealed class ConsumeJiraAuthorizationContext : JiraAuthorizationContext
        {
            public Task<string> Authorization { get; private set; }

            public ConsumeJiraAuthorizationContext(BasicAuthOptions options)
            {
                var bytes = Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}");
                var authorization = Convert.ToBase64String(bytes);

                Authorization = Task.FromResult(authorization);
            }
        }
    }
}
