using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.JiraServicedeskConnector.Contexts;
using MassTransit.JiraServicedeskConnector.Contexts.Commands;
using MassTransit.JiraServicedeskConnector.Contexts.Queries;
using MassTransit.JiraServicedeskConnector.Contexts.Results;
using MassTransit.JiraServicedeskConnector.Options;
using MassTransit.JiraServicedeskConnector;
using MassTransit.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace MassTransit.JiraServicedeskConnector.Pipeline.Filters
{
    public sealed class JiraServicedeskFilter<TContext> : IFilter<TContext>
        where TContext : class, ConsumeContext
    {
        static readonly ILog _log = Logger.Get<JiraServicedeskFilter<TContext>>();

        readonly ServerOptions _serverOptions;

        readonly BehaviorOptions _behaviorOptions;

        readonly IJiraServicedeskApiClient _client;

        public JiraServicedeskFilter(ServerOptions serverOptions, BehaviorOptions behaviorOptions)
        {
            _serverOptions = serverOptions;
            _behaviorOptions = behaviorOptions;

            _client = RestService.For<IJiraServicedeskApiClient>(_serverOptions.BaseAddress, new RefitSettings
            {
                ContentSerializer = new JsonContentSerializer(new JsonSerializerSettings
                {
                    ContractResolver = new PrefixContractResolver(new CamelCasePropertyNamesContractResolver())
                })
            });
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope(nameof(JiraServicedeskFilter<TContext>));
            scope.CreateScope(nameof(ServerOptions)).Set(_serverOptions);
            scope.CreateScope(nameof(BehaviorOptions)).Set(_behaviorOptions);
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            _log.Debug(() => "Sending through filter.");

            var jiraAuthorizationContext = context.GetPayload<JiraAuthorizationContext>();

            var authorization = await jiraAuthorizationContext.Authorization.ConfigureAwait(false);

            var jiraServicedeskContext = new ConsumeJiraServicedeskContext(context, _client, authorization);

            context.GetOrAddPayload(() => jiraServicedeskContext);

            await next.Send(context).ConfigureAwait(false);
        }

        sealed class ConsumeJiraServicedeskContext : JiraServicedeskContext
        {
            readonly TContext _context;

            readonly IJiraServicedeskApiClient _client;

            readonly string _authorization;

            public ConsumeJiraServicedeskContext(TContext context, IJiraServicedeskApiClient client, string authorization)
            {
                _context = context;
                _client = client;
                _authorization = authorization;
            }

            public async Task<CreateCustomerRequestResult> Send(CreateCustomerRequestCommand command, CancellationToken cancellationToken)
            {
                CreateCustomerRequestModel createCustomerRequestModel = new CreateCustomerRequestModel
                {
                    ServiceDeskId = command.ServiceDeskId,
                    RequestTypeId = command.RequestTypeId,
                    RequestFieldValues =
                    {
                        Description = command.Description,
                        Summary = command.Summary
                    }
                };

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(_context.CancellationToken, cancellationToken);

                var createdCustomerRequestModel = await _client.CreateCustomerRequest(_authorization, createCustomerRequestModel, cts.Token).ConfigureAwait(false);

                return new CreateCustomerRequestResult
                {
                    IssueId = createdCustomerRequestModel.IssueId,
                    IssueKey = createdCustomerRequestModel.IssueKey,
                    WebLink = createdCustomerRequestModel.Links.Web
                };
            }

            public async Task Send(CustomerRequestQuery query, CancellationToken cancellationToken)
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(_context.CancellationToken, cancellationToken);

                await _client.GetCustomerRequestByIdOrKey(_authorization, query.IssueIdOrKey, cts.Token).ConfigureAwait(false);
            }

            public async Task Send(MyCustomerRequestsQuery query, CancellationToken cancellationToken)
            {
                GetMyCustomerRequestsModel getMyCustomerRequestsModel = new GetMyCustomerRequestsModel
                {
                    RequestTypeId = query.RequestTypeId,
                    ServiceDeskId = query.ServiceDeskId,
                    SearchTerm = query.SearchTerm,
                    Limit = query.Limit,
                    Start = query.Start
                };

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(_context.CancellationToken, cancellationToken);

                var myCustomerRequests = await _client.GetMyCustomerRequests(_authorization, getMyCustomerRequestsModel, cts.Token).ConfigureAwait(false);
            }

            public async Task<CreateRequestCommentResult> Send(CreateRequestCommentCommand command, CancellationToken cancellationToken)
            {
                var createRequestCommandModel = new CreateRequestCommentModel
                {
                    Comment = command.Comment,
                    Public = command.Public
                };

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(_context.CancellationToken, cancellationToken);

                var createdRequestCommentModel = await _client.CreateRequestComment(_authorization, command.IssueIdOrKey, createRequestCommandModel, cts.Token).ConfigureAwait(false);

                return new CreateRequestCommentResult { Id = createdRequestCommentModel.Id };
            }
        }
    }
}
