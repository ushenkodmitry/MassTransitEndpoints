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
    [Category("SmtpStorage, Consumers")]
    [TestFixture]
    public sealed class CreateSmtpInstanceConsumer_Specs
    {
        InMemoryTestHarness _harness;

        Mock<ISmtpInstancesRepository> _smtpInstancesRepositoryMock;

        Mock<IDocumentSession> _documentSessionMock;

        CreateSmtpInstance _createSmtpInstance;

        CreateSmtpInstanceCommand _createSmtpInstanceCommand;

        const int _id = 1000;

        [SetUp]
        public async Task A_consumer_being_tested()
        {
            _smtpInstancesRepositoryMock = new Mock<ISmtpInstancesRepository>();
            _smtpInstancesRepositoryMock
                .Setup(x => x.SendCommand(IsAny<PipeContext>(), IsAny<CreateSmtpInstanceCommand>(), IsAny<CancellationToken>()))
                .Callback<PipeContext, CreateSmtpInstanceCommand, CancellationToken>((context, command, __) =>
                {
                    _createSmtpInstanceCommand = command;

                    var identity = context.GetOrAddPayload(() => new Identity<SmtpInstance, int>(_id));
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

            var sut = _harness.Consumer(() => new CreateSmtpInstanceConsumer(_smtpInstancesRepositoryMock.Object));

            _createSmtpInstance = TypeCache<CreateSmtpInstance>.InitializeFromObject(new
            {
                Name = Guid.NewGuid().ToString(),
                SmtpConnectionId = 1000,
                UserCredentialsId = 100,
                InstancesCount = 2
            });

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(_createSmtpInstance);
        }

        [TearDown]
        public async Task Before_each()
        {
            _smtpInstancesRepositoryMock.Reset();

            await _harness.Stop();
        }

        [Test]
        public void Should_store_smtp_instance()
        {
            //
            var consumed = _harness.Consumed.Select<CreateSmtpInstance>().Single();

            //
            _createSmtpInstanceCommand.Should().BeEquivalentTo(_createSmtpInstance);
        }

        [Test]
        public void Should_publish_smtp_instance_created()
        {
            //
            var published = _harness.Published.Select<SmtpInstanceCreated>().Single();

            //
            published.Context.Message.Id.Should().Be(_id);
        }
    }
}
