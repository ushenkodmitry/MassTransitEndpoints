using GreenPipes;
using MassTransit.JiraServicedeskConnector.Contexts;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MassTransit.JiraServicedeskConnector.Consumers
{
    [Category("JiraServicedeskConnector")]
    [TestFixture]
    public class CreateRequestCommentConsumer_Specs
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
    }
}