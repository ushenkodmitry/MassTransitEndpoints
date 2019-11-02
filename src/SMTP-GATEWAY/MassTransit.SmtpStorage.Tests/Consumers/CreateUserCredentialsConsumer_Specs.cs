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
    public sealed class CreateUserCredentialsConsumer_Specs
    {
        InMemoryTestHarness _harness;

        Mock<IUserCredentialsRepository> _userCredentialsRepositoryMock;

        Mock<IDocumentSession> _documentSessionMock;

        CreateUserCredentials _createUserCredentials;

        CreateUserCredentialsCommand _createUserCredentialsCommand;

        const int _id = 1000;

        [SetUp]
        public async Task A_consumer_being_tested()
        {
            _userCredentialsRepositoryMock = new Mock<IUserCredentialsRepository>();
            _userCredentialsRepositoryMock
                .Setup(x => x.SendCommand(IsAny<PipeContext>(), IsAny<CreateUserCredentialsCommand>(), IsAny<CancellationToken>()))
                .Callback<PipeContext, CreateUserCredentialsCommand, CancellationToken>((context, command, __) =>
                {
                    _createUserCredentialsCommand = command;

                    _ = context.GetOrAddPayload(() => new Identity<UserCredentials, int>(_id));
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

            var sut = _harness.Consumer(() => new CreateUserCredentialsConsumer(_userCredentialsRepositoryMock.Object));

            _createUserCredentials = TypeCache<CreateUserCredentials>.InitializeFromObject(new
            {
                UserName = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString()
            });

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(_createUserCredentials);
        }

        [TearDown]
        public async Task Before_each()
        {
            _userCredentialsRepositoryMock.Reset();

            await _harness.Stop();
        }

        [Test]
        public void Should_store_user_credentials_once()
        {
            //
            _ = _harness.Consumed.Select<CreateUserCredentials>().Single();

            //
            _createUserCredentialsCommand.Should().BeEquivalentTo(_createUserCredentials);
        }

        [Test]
        public void Should_publish_user_credentials_created()
        {
            //
            var published = _harness.Published.Select<UserCredentialsCreated>().Single();

            //
            published.Context.Message.Id.Should().Be(_id);
        }
    }
}
