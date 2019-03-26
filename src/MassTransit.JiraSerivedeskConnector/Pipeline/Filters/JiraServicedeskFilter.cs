using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.JiraSerivedeskConnector.Contexts;
using MassTransit.Logging;

namespace MassTransit.JiraSerivedeskConnector.Pipeline.Filters
{
    public sealed class JiraServicedeskFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<JiraServicedeskFilter<TContext>>();

        readonly ServerOptions _options;

        public JiraServicedeskFilter(ServerOptions options) => _options = options;

        public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(JiraServicedeskFilter<TContext>)).Set(_options);

        public Task Send(TContext context, IPipe<TContext> next)
        {
            _log.Debug(() => "Sending through filter.");

            OptionsContext optionsContext = context.GetPayload<OptionsContext>();


            throw new System.NotImplementedException();
        }

        sealed class ConsumeJiraServicedeskContext : JiraServicedeskContext
        {
            public Task AuthenticationCompleted { get; private set; }


        }

    }
}
