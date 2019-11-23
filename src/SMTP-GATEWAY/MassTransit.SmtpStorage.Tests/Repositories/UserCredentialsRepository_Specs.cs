using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GreenPipes;
using Marten;
using MassTransit.Contexts;
using MassTransit.Objects.Commands;
using MassTransit.Objects.Models;
using MassTransit.Payloads;
using Moq;
using NUnit.Framework;
using static Moq.It;
using static Moq.Mock;

namespace MassTransit.Repositories
{
    [Category("SmtpStorage, Repositories")]
    [TestFixture]
    public class UserCredentialsRepository_Specs
    {
        UserCredentialsRepository _sut;

        Mock<PipeContext> _contextMock;

        Mock<IDocumentSession> _documentSessionMock;

        [OneTimeSetUp]
        public void A_repository_being_tested()
        {
            _documentSessionMock = new Mock<IDocumentSession>();

            var documentStoreContext = Of<DocumentStoreContext>(x =>
                x.OpenSession(IsAny<string>(), IsAny<IsolationLevel>()) == new ValueTask<IDocumentSession>(_documentSessionMock.Object));

            _contextMock = new Mock<PipeContext>();
            _contextMock
                .Setup(x => x.TryGetPayload(out documentStoreContext))
                .Returns(true);

            _sut = new UserCredentialsRepository();
        }

        [SetUp]
        public void Before_each() => _documentSessionMock.Reset();

        [Test]
        public async Task Should_store_user_credentials()
        {
            //
            const int id = 100;

            var createUserCredentialsCommand = new CreateUserCredentialsCommand
            {
                Password = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString()
            };

            UserCredentials userCredentials = null;
            _documentSessionMock
                .Setup(x => x.Insert(IsAny<UserCredentials>()))
                .Callback<UserCredentials[]>(credentials =>
                {
                    userCredentials = credentials[0];
                    userCredentials.Id = id;
                });

            //
            await _sut.SendCommand(_contextMock.Object, createUserCredentialsCommand, CancellationToken.None);

            //
            userCredentials.Should().BeEquivalentTo(createUserCredentialsCommand, opts => opts.ExcludingMissingMembers());
        }

        [Test]
        public async Task Should_set_id_on_createdid_payload()
        {
            //
            const int id = 100;

            CreatedId<UserCredentials, int> createdId = new CreatedId<UserCredentials, int>();

            _contextMock
                .Setup(x => x.TryGetPayload(out createdId))
                .Returns(true);

            _documentSessionMock
                .Setup(x => x.Insert(IsAny<UserCredentials>()))
                .Callback<UserCredentials[]>(credentials =>
                {
                    var userCredentials = credentials[0];
                    userCredentials.Id = id;
                });

            //
            await _sut.SendCommand(_contextMock.Object, new CreateUserCredentialsCommand(), CancellationToken.None);

            //
            createdId.Id.Should().Be(id);
        }
    }
}
