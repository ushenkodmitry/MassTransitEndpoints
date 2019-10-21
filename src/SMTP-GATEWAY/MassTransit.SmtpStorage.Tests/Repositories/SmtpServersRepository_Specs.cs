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
    public class SmtpServersRepository_Specs
    {
        SmtpServersRepository _sut;

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

            _sut = new SmtpServersRepository();
        }

        [SetUp]
        public void Before_each() => _documentSessionMock.Reset();

        [Test]
        public async Task Should_store_smtp_server()
        {
            //
            const int id = 100;

            var createSmtpServerCommand = new CreateSmtpServerCommand
            {
                Host = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Port = 1000,
                UseSsl = true
            };

            SmtpServer smtpServer = null;
            _documentSessionMock
                .Setup(x => x.Insert(IsAny<SmtpServer>()))
                .Callback<SmtpServer[]>(servers =>
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

            PayloadFactory<Identity<SmtpServer, int>> payloadFactory = null;
            _contextMock
                .Setup(x => x.AddOrUpdatePayload(IsAny<PayloadFactory<Identity<SmtpServer, int>>>(), IsAny<UpdatePayloadFactory<Identity<SmtpServer, int>>>()))
                .Callback<PayloadFactory<Identity<SmtpServer, int>>, UpdatePayloadFactory<Identity<SmtpServer, int>>>((factory, _) => payloadFactory = factory);

            _documentSessionMock
                .Setup(x => x.Insert(IsAny<SmtpServer>()))
                .Callback<SmtpServer[]>(servers =>
                {
                    var smtpServer = servers[0];
                    smtpServer.Id = id;
                });

            //
            await _sut.SendCommand(_contextMock.Object, new CreateSmtpServerCommand(), CancellationToken.None);

            //
            var payload = payloadFactory();
            payload.Id.Should().Be(id);
        }
    }
}
