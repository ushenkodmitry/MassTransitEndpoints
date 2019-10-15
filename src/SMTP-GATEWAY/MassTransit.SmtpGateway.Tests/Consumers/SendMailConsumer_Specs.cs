using GreenPipes;
using GreenPipes.Internals.Extensions;
using MassTransit.SmtpGateway.Consumers;
using MassTransit.SmtpGateway.Contexts;
using MassTransit.SmtpGateway.Messages;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Moq.Times;
using static Moq.It;
using MimeKit;
using System.Threading;
using System.Text;

namespace MassTransit.SmtpGateway
{
    [Category("SmtpGateway")]
    [TestFixture]
    public class SendMailConsumer_Specs
    {
        InMemoryTestHarness _harness;

        Mock<SmtpContext> _smtpContextMock;

        [OneTimeSetUp]
        public void A_consumer_being_tested()
        {
            _smtpContextMock = new Mock<SmtpContext>();

            _harness = new InMemoryTestHarness { TestTimeout = TimeSpan.FromSeconds(5) };
            _harness.OnConfigureReceiveEndpoint += configurator =>
            {
                configurator.UseInlineFilter((context, next) =>
                {
                    context.GetOrAddPayload(() => _smtpContextMock.Object);

                    return next.Send(context);
                });
            };

            var sut = _harness.Consumer(() => new SendMailConsumer());
        }

        [SetUp]
        public Task SetUp() => _harness.Start();

        [TearDown]
        public Task TearDown() => _harness.Stop();

        [Test]
        public async Task Should_send_via_smtp_context()
        {
            //
            SendMail sendMail = TypeCache<SendMail>.InitializeFromObject(new
            {
                Subject = "sub ject",
                Importance = "Normal",
                XPriority = "Low",
                MessageId = NewId.NextGuid().ToString(),
                TextBody = "text body",
                From = new [] { $"me{ASCII.UnitSeparator}me@mail.com" },
                To = new[] { $"me{ASCII.UnitSeparator}me@mail.com" },
                Priority = "Normal",
                AttachmentsMeta = Array.Empty<string>()
            });

            //
            await _harness.InputQueueSendEndpoint.Send(sendMail);
            var consumed = _harness.Consumed.Select<SendMail>().First();

            //
            _smtpContextMock
                .Verify(x => x.Send(IsAny<MimeMessage>(), IsAny<CancellationToken>()), Once);
        }
    }
}