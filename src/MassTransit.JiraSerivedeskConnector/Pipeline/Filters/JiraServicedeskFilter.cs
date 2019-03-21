using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.JiraSerivedeskConnector.Configuration;
using MassTransit.JiraSerivedeskConnector.Contexts;
using MassTransit.Logging;

namespace MassTransit.JiraSerivedeskConnector.Pipeline.Filters
{
    public sealed class JiraServicedeskFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<JiraServicedeskFilter<TContext>>();

        readonly ServerOptions _serverOptions;

        public JiraServicedeskFilter(Action<IJiraServicedeskConfigurator> configureJiraServicedesk)
        {
            var configurator = new JiraServicedeskConfigurator();
            configureJiraServicedesk(configurator);
            _serverOptions = configurator.Options;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(JiraServicedeskFilter<TContext>));
            scope.Set(_serverOptions);
        }

        public Task Send(TContext context, IPipe<TContext> next)
        {
            throw new System.NotImplementedException();
        }

        sealed class ConsumeJiraServicedeskContext : JiraServicedeskContext
        {
            public Task AuthenticationCompleted { get; private set; }


        }

    }
}
