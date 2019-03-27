using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.JiraServicedeskConnector
{
    interface IJiraServicedeskApiClient
    {
        [Post("/rest/servicedeskapi/request")]
        Task<CustomerRequestModel> CreateCustomerRequest([Header("Authorization")] string authorization, [Body] CreateCustomerRequestModel model, CancellationToken cancellationToken = default);

        [Get("/rest/servicedeskapi/request")]
        Task<CustomerRequestsModel> GetMyCustomerRequests([Header("Authorization")] string authorization, [Query] GetMyCustomerRequestsModel model, CancellationToken cancellationToken = default);

        [Get("/rest/servicedeskapi/request/{issueIdOrKey}")]
        Task GetCustomerRequestByIdOrKey([Header("Authorization")] string authorization, string issueIdOrKey, CancellationToken cancellationToken = default);

        [Post("/rest/servicedeskapi/request/{issueIdOrKey}/comment")]
        Task<CreatedRequestCommentModel> CreateRequestComment([Header("Authorization")] string authorization, string issueIdOrKey, [Body] CreateRequestCommentModel model, CancellationToken cancellationToken = default);

        [Post("/rest/servicedeskapi/request/{issueIdOrKey}/participant")]
        Task<RequestParticipantsModel> GetRequestParticipants([Header("Authorization")] string authorization, string issueIdOrKey, [Query] int start, [Query] int limit, CancellationToken cancellationToken = default);

        [Post("/rest/servicedeskapi/request/{issueIdOrKey}/participant")]
        Task AddRequestParticipants([Header("Authorization")] string authorization, string issueIdOrKey, [Body] ModifyRequestParticipantsModel model, CancellationToken cancellationToken = default);

        [Delete("/rest/servicedeskapi/request/{issueIdOrKey}/participant")]
        Task RemoveRequestParticipants([Header("Authorization")] string authorization, string issueIdOrKey, [Body] ModifyRequestParticipantsModel model, CancellationToken cancellationToken = default);

        [Get("/rest/servicedeskapi/servicedesk")]
        Task<ServicedesksModel> GetServicedesks([Header("Authorization")] string authorization, CancellationToken cancellationToken = default);

        [Get("/rest/servicedeskapi/servicedesk/{servicedeskId}")]
        Task<ServicedeskModel> GetServicedeskById([Header("Authorization")] string authorization, string servicedeskId, CancellationToken cancellationToken = default);
    }
}
