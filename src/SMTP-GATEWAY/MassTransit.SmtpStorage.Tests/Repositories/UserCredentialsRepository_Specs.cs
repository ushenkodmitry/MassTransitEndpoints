using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Contexts;
using MassTransit.Objects.Commands;
using Moq;
using NUnit.Framework;
using static Moq.Mock;
using static Moq.It;
using Marten;
using System.Data;
using MassTransit.Objects.Models;
using static Moq.Times;
using FluentAssertions;
using MassTransit.Payloads;

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
            _documentSessionMock
                .Verify(x => x.SaveChangesAsync(IsAny<CancellationToken>()), Once);
            userCredentials.Should().BeEquivalentTo(createUserCredentialsCommand, opts => opts.ExcludingMissingMembers());
        }

        [Test]
        public async Task Should_add_identity_payload()
        {
            //
            const int id = 100;

            PayloadFactory<Identity<UserCredentials, int>> payloadFactory = null;
            _contextMock
                .Setup(x => x.AddOrUpdatePayload(IsAny<PayloadFactory<Identity<UserCredentials, int>>>(), IsAny<UpdatePayloadFactory<Identity<UserCredentials, int>>>()))
                .Callback<PayloadFactory<Identity<UserCredentials, int>>, UpdatePayloadFactory<Identity<UserCredentials, int>>>((factory, _) => payloadFactory = factory);

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
            var payload = payloadFactory();
            payload.Id.Should().Be(id);
        }
    }
}
