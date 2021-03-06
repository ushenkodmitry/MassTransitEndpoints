﻿using GreenPipes;
using MassTransit.JiraServicedeskConnector.Contexts;
using MassTransit.JiraServicedeskConnector.Options;
using MassTransit.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.JiraServicedeskConnector.Pipeline.Filters
{
    public sealed class JiraBasicAuthFilter<TContext> : IFilter<TContext>
        where TContext : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<JiraBasicAuthFilter<TContext>>();

        readonly Task<string> _authorization;

        public JiraBasicAuthFilter(BasicAuthOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}");
            var authorization = $"Basic {Convert.ToBase64String(bytes)}";
            _authorization = Task.FromResult(authorization);
        }

        public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(JiraBasicAuthFilter<TContext>)).Add("Authorization", _authorization);

        public Task Send(TContext context, IPipe<TContext> next)
        {
            _log.Debug(() => "Sending through filter.");

            JiraAuthorizationContext jiraAuthorizationContext = new ConsumeJiraAuthorizationContext(_authorization);

            context.GetOrAddPayload(() => jiraAuthorizationContext);

            return next.Send(context);
        }

        sealed class ConsumeJiraAuthorizationContext : JiraAuthorizationContext
        {
            public Task<string> Authorization { get; private set; }

            public ConsumeJiraAuthorizationContext(Task<string> authorization) => Authorization = authorization;
        }
    }
}
