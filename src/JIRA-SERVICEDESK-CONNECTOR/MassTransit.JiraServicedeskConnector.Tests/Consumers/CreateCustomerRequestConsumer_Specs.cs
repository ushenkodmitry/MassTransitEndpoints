using GreenPipes;
using GreenPipes.Internals.Extensions;
using MassTransit.JiraServicedeskConnector.Contexts;
using MassTransit.JiraServicedeskConnector.Contexts.Commands;
using MassTransit.JiraServicedeskConnector.Messages;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Moq.Mock;
using static Moq.It;
using FluentAssertions;
using MassTransit.JiraServicedeskConnector.Contexts.Results;

namespace MassTransit.JiraServicedeskConnector.Consumers
{
    [Category("JiraServicedeskConnector")]
    [TestFixture]
    public class CreateCustomerRequestConsumer_Specs
    {
        InMemoryTestHarness _harness;

        Mock<JiraServicedeskContext> _jiraServicedeskContextMock;

        [OneTimeSetUp]
        public void A_consumer_being_tested()
        {
            _jiraServicedeskContextMock = new Mock<JiraServicedeskContext>();

            _harness = new InMemoryTestHarness { TestTimeout = TimeSpan.FromSeconds(5) };
            _harness.OnConfigureReceiveEndpoint += configurator =>
            {
                configurator.UseInlineFilter((context, next) =>
                {
                    context.GetOrAddPayload(() => _jiraServicedeskContextMock.Object);

                    return next.Send(context);
                });
            };

            var sut = _harness.Consumer(() => new CreateCustomerRequestConsumer());
        }

        [SetUp]
        public Task SetUp() => _harness.Start();

        [TearDown]
        public Task TearDown() => _harness.Stop();

        [Test]
        public async Task Should_send_create_customer_request_command_using_jira_servicedesk_context()
        {
            //
            var createCustomerRequest = TypeCache<CreateCustomerRequest>.InitializeFromObject(new
            {
                ServiceDeskId = NewId.NextGuid().ToString(),
                RequestTypeId = NewId.NextGuid().ToString(),
                Summary = NewId.NextGuid().ToString(),
                Description = NewId.NextGuid().ToString()
            });

            CreateCustomerRequestCommand createCustomerRequestCommand = null;
            _jiraServicedeskContextMock
                .Setup(x => x.Send(IsAny<CreateCustomerRequestCommand>(), IsAny<CancellationToken>()))
                .Callback<CreateCustomerRequestCommand, CancellationToken>((command, _) => createCustomerRequestCommand = command);

            //
            await _harness.InputQueueSendEndpoint.Send(createCustomerRequest);
            var consumed = _harness.Consumed.Select<CreateCustomerRequest>().First();

            //
            createCustomerRequestCommand.Should().NotBeNull();
            createCustomerRequestCommand.Should().BeEquivalentTo(createCustomerRequest, opts => opts.ExcludingMissingMembers());
        }

        [Test]
        public async Task Should_publish_customer_request_created()
        {
            //
            var createCustomerRequest = TypeCache<CreateCustomerRequest>.InitializeFromObject(new
            {
                ServiceDeskId = NewId.NextGuid().ToString(),
                RequestTypeId = NewId.NextGuid().ToString(),
                Summary = NewId.NextGuid().ToString(),
                Description = NewId.NextGuid().ToString(),
                CorrelationId = NewId.NextGuid()
            });

            var createCustomerRequestResult = new CreateCustomerRequestResult
            {
                IssueId = NewId.NextGuid().ToString(),
                IssueKey = NewId.NextGuid().ToString(),
                WebLink = NewId.NextGuid().ToString()
            };

            _jiraServicedeskContextMock
                .Setup(x => x.Send(IsAny<CreateCustomerRequestCommand>(), IsAny<CancellationToken>()))
                .ReturnsAsync(createCustomerRequestResult);

            //
            await _harness.InputQueueSendEndpoint.Send(createCustomerRequest);
            var published = _harness.Published.Select<CustomerRequestCreated>().First();

            //
            published.Context.Message.Should().BeEquivalentTo(createCustomerRequest, opts => opts.ExcludingMissingMembers());
            published.Context.Message.Should().BeEquivalentTo(createCustomerRequestResult, opts => opts.ExcludingMissingMembers());

        }
    }
}