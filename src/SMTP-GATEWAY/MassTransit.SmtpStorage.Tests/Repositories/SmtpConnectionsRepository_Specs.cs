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
using static Moq.Times;

namespace MassTransit.Repositories
{
    [Category("SmtpStorage, Repositories")]
    [TestFixture]
    public class SmtpConnectionsRepository_Specs
    {
        SmtpConnectionsRepository _sut;

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

            _sut = new SmtpConnectionsRepository();
        }

        [SetUp]
        public void Before_each() => _documentSessionMock.Reset();

        [Test]
        public async Task Should_store_smtp_server()
        {
            //
            const int id = 100;

            var createSmtpServerCommand = new CreateSmtpConnectionCommand
            {
                Host = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Port = 1000,
                UseSsl = true
            };

            SmtpConnection smtpServer = null;
            _documentSessionMock
                .Setup(x => x.Insert(IsAny<SmtpConnection>()))
                .Callback<SmtpConnection[]>(servers =>
                {
                    smtpServer = servers[0];
                    smtpServer.Id = id;
                });

            //
            await _sut.SendCommand(_contextMock.Object, createSmtpServerCommand, CancellationToken.None);

            //
            _documentSessionMock
                .Verify(x => x.SaveChangesAsync(IsAny<CancellationToken>()), Once);
            smtpServer.Should().BeEquivalentTo(createSmtpServerCommand, opts => opts.ExcludingMissingMembers());
        }

        [Test]
        public async Task Should_add_identity_payload()
        {
            //
            const int id = 100;

            PayloadFactory<Identity<SmtpConnection, int>> payloadFactory = null;
            _contextMock
                .Setup(x => x.AddOrUpdatePayload(IsAny<PayloadFactory<Identity<SmtpConnection, int>>>(), IsAny<UpdatePayloadFactory<Identity<SmtpConnection, int>>>()))
                .Callback<PayloadFactory<Identity<SmtpConnection, int>>, UpdatePayloadFactory<Identity<SmtpConnection, int>>>((factory, _) => payloadFactory = factory);

            _documentSessionMock
                .Setup(x => x.Insert(IsAny<SmtpConnection>()))
                .Callback<SmtpConnection[]>(servers =>
                {
                    var smtpServer = servers[0];
                    smtpServer.Id = id;
                });

            //
            await _sut.SendCommand(_contextMock.Object, new CreateSmtpConnectionCommand(), CancellationToken.None);

            //
            var payload = payloadFactory();
            payload.Id.Should().Be(id);
        }
    }
}
