using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GreenPipes;
using GreenPipes.Internals.Extensions;
using Marten;
using MassTransit.Contexts;
using MassTransit.Messages;
using MassTransit.Objects.Commands;
using MassTransit.Objects.Models;
using MassTransit.Payloads;
using MassTransit.Repositories;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using static Moq.It;
using static Moq.Mock;

namespace MassTransit.Consumers
{
    [Category("Consumer, SmtpStorage")]
    [TestFixture]
    public sealed class CreateSmtpConnectionConsumer_Specs
    {
        InMemoryTestHarness _harness;

        Mock<ISmtpConnectionsRepository> _smtpServersRepositoryMock;

        Mock<IDocumentSession> _documentSessionMock;

        CreateSmtpConnection _createSmtpConnection;

        CreateSmtpConnectionCommand _createSmtpServerCommand;

        const int _id = 1000;

        [SetUp]
        public async Task A_consumer_being_tested()
        {
            _smtpServersRepositoryMock = new Mock<ISmtpConnectionsRepository>();
            _smtpServersRepositoryMock
                .Setup(x => x.SendCommand(IsAny<PipeContext>(), IsAny<CreateSmtpConnectionCommand>(), IsAny<CancellationToken>()))
                .Callback<PipeContext, CreateSmtpConnectionCommand, CancellationToken>((context, command, __) =>
                {
                    _createSmtpServerCommand = command;

                    var identity = context.GetOrAddPayload(() => new Identity<SmtpConnection, int>(_id));
                })
                .Returns(Task.CompletedTask);

            _documentSessionMock = new Mock<IDocumentSession>();

            var documentStoreContext = Of<DocumentStoreContext>(x =>
                x.LightweightSession(IsAny<string>(), IsAny<IsolationLevel>()) == new ValueTask<IDocumentSession>(_documentSessionMock.Object) &&
                x.OpenSession(IsAny<string>(), IsAny<IsolationLevel>()) == new ValueTask<IDocumentSession>(_documentSessionMock.Object));

            _harness = new InMemoryTestHarness { TestTimeout = TimeSpan.FromSeconds(2) };
            _harness.OnConfigureReceiveEndpoint += configurator =>
            {
                configurator.UseInlineFilter((context, next) =>
                {
                    context.GetOrAddPayload(() => documentStoreContext);

                    return next.Send(context);
                });
            };

            var sut = _harness.Consumer(() => new CreateSmtpConnectionConsumer(_smtpServersRepositoryMock.Object));

            _createSmtpConnection = TypeCache<CreateSmtpConnection>.InitializeFromObject(new
            {
                Name = Guid.NewGuid().ToString(),
                Host = "host.com",
                Port = 1000,
                UseSsl = true
            });

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(_createSmtpConnection);
        }

        [TearDown]
        public async Task Before_each()
        {
            _smtpServersRepositoryMock.Reset();

            await _harness.Stop();
        }

        [Test]
        public void Should_store_smtp_connection_once()
        {
            //
            var consumed = _harness.Consumed.Select<CreateSmtpConnection>().Single();

            //
            _createSmtpServerCommand.Should().BeEquivalentTo(_createSmtpConnection);
        }

        [Test]
        public void Should_publish_smtp_connection_created()
        {
            //
            var published = _harness.Published.Select<SmtpConnectionCreated>().Single();

            //
            published.Context.Message.Id.Should().Be(_id);
        }
    }
}
