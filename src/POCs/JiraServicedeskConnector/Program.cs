using MassTransit;
using MassTransit.JiraServicedeskConnector.Messages;
using System;
using System.Threading.Tasks;
using MassTransit.JiraServicedeskConnector.Configuration;
using MassTransit.JiraServicedeskConnector.Options;

namespace JiraServicedeskConnector
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(inMemory =>
            {
                inMemory.ReceiveEndpoint(endpoint =>
                {
                    endpoint.UseJiraServicedesk(jira =>
                    {
                        jira.UseOptions((ServerOptions options) =>
                        {
                            options.BaseAddress = "";
                        });
                        jira.UseOptions((BehaviorOptions options) =>
                        {
                        });
                        jira.UseOptions((BasicAuthOptions options) =>
                        {
                            options.Username = "";
                            options.Password = "";
                        });
                    });
                });
            });

            await bus.StartAsync();

            await bus.Publish<CreateCustomerRequest>(new
            {
                ServiceDeskId = "4",
                RequestTypeId = "26",
                Description = "Test",
                Summary = "Test1"
            });

            await bus.StopAsync();

            await Console.In.ReadLineAsync();
        }
    }
}
