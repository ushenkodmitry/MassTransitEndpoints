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
    public sealed class CreateSmtpServerConsumer_Specs
    {
        InMemoryTestHarness _harness;

        Mock<ISmtpServersRepository> _smtpServersRepositoryMock;

        Mock<IDocumentSession> _documentSessionMock;

        CreateSmtpServer _createSmtpServer;

        [OneTimeSetUp]
        public async Task A_consumer_being_tested()
        {
            _smtpServersRepositoryMock = new Mock<ISmtpServersRepository>();

            _documentSessionMock = new Mock<IDocumentSession>();

            var documentStoreContext = Of<DocumentStoreContext>(x =>
                x.LightweightSession(IsAny<string>(), IsAny<IsolationLevel>()) == new ValueTask<IDocumentSession>(_documentSessionMock.Object) &&
                x.OpenSession(IsAny<string>(), IsAny<IsolationLevel>()) == new ValueTask<IDocumentSession>(_documentSessionMock.Object));

            _harness = new InMemoryTestHarness { TestTimeout = TimeSpan.FromSeconds(5) };
            _harness.OnConfigureReceiveEndpoint += configurator =>
            {
                configurator.UseInlineFilter((context, next) =>
                {
                    context.GetOrAddPayload(() => documentStoreContext);

                    return next.Send(context);
                });
            };

            var sut = _harness.Consumer(() => new CreateSmtpServerConsumer(_smtpServersRepositoryMock.Object));

            _createSmtpServer = TypeCache<CreateSmtpServer>.InitializeFromObject(new
            {
                Name = Guid.NewGuid().ToString(),
                Host = "host.com",
                Port = 1000,
                UseSsl = true
            });

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(_createSmtpServer);
        }

        [SetUp]
        public void Before_each() => _smtpServersRepositoryMock.Reset();

        [Test]
        public void Should_store_smtp_server_once()
        {
            //
            CreateSmtpServerCommand createSmtpServerCommand = null;
            _smtpServersRepositoryMock
                .Setup(x => x.SendCommand(IsAny<PipeContext>(), IsAny<CreateSmtpServerCommand>(), IsAny<CancellationToken>()))
                .Callback<PipeContext, CreateSmtpServerCommand, CancellationToken>((context, command, __) =>
                {
                    createSmtpServerCommand = command;

                    var identity = context.GetOrAddPayload(() => new Identity<SmtpServer, int>());
                })
                .Returns(Task.CompletedTask);

            //
            var consumed = _harness.Consumed.Select<CreateSmtpServer>().Single();

            //
            createSmtpServerCommand.Should().BeEquivalentTo(_createSmtpServer);
        }

        [Test]
        public void Should_publish_smtp_server_created()
        {
            //
            const int id = 1000;

            _smtpServersRepositoryMock
                .Setup(x => x.SendCommand(IsAny<PipeContext>(), IsAny<CreateSmtpServerCommand>(), IsAny<CancellationToken>()))
                .Callback<PipeContext, CreateSmtpServerCommand, CancellationToken>((context, __, ___) =>
                {
                    var identity = context.AddOrUpdatePayload(() => new Identity<SmtpServer, int>(id), (identity) => new Identity<SmtpServer, int>(id));
                })
                .Returns(Task.CompletedTask);

            //
            var published = _harness.Published.Select<SmtpServerCreated>().Single();

            //
            published.Context.Message.Id.Should().Be(id);
        }
    }
}
