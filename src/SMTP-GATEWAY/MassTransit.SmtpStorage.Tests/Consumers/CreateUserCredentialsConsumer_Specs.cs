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
    public sealed class CreateUserCredentialsConsumer_Specs
    {
        InMemoryTestHarness _harness;

        Mock<IUserCredentialsRepository> _userCredentialsRepositoryMock;

        Mock<IDocumentSession> _documentSessionMock;

        CreateUserCredentials _createUserCredentials;

        [OneTimeSetUp]
        public async Task A_consumer_being_tested()
        {
            _userCredentialsRepositoryMock = new Mock<IUserCredentialsRepository>();

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

            var sut = _harness.Consumer(() => new CreateUserCredentialsConsumer(_userCredentialsRepositoryMock.Object));

            _createUserCredentials = TypeCache<CreateUserCredentials>.InitializeFromObject(new
            {
                Name = Guid.NewGuid().ToString(),
                Host = "host.com",
                Port = 1000,
                UseSsl = true
            });

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(_createUserCredentials);
        }

        [SetUp]
        public void Before_each() => _userCredentialsRepositoryMock.Reset();

        [Test]
        public void Should_store_user_credentials_once()
        {
            //
            CreateUserCredentialsCommand createUserCredentialsCommand = null;
            _userCredentialsRepositoryMock
                .Setup(x => x.SendCommand(IsAny<PipeContext>(), IsAny<CreateUserCredentialsCommand>(), IsAny<CancellationToken>()))
                .Callback<PipeContext, CreateUserCredentialsCommand, CancellationToken>((context, command, __) =>
                {
                    createUserCredentialsCommand = command;

                    var identity = context.GetOrAddPayload(() => new Identity<UserCredentials, int>(1000));
                })
                .Returns(Task.CompletedTask);

            //
            var consumed = _harness.Consumed.Select<CreateUserCredentials>().Single();

            //
            createUserCredentialsCommand.Should().BeEquivalentTo(_createUserCredentials);
        }

        [Test]
        public void Should_publish_user_credentials_created()
        {
            //
            const int id = 1000;

            _userCredentialsRepositoryMock
                .Setup(x => x.SendCommand(IsAny<PipeContext>(), IsAny<CreateUserCredentialsCommand>(), IsAny<CancellationToken>()))
                .Callback<PipeContext, CreateUserCredentialsCommand, CancellationToken>((context, __, ___) =>
                {
                    var identity = context.AddOrUpdatePayload(() => new Identity<UserCredentials, int>(id), (identity) => new Identity<UserCredentials, int>(id));
                })
                .Returns(Task.CompletedTask);

            //
            var published = _harness.Published.Select<SmtpServerCreated>().Single();

            //
            published.Context.Message.Id.Should().Be(id);
        }
    }
}
