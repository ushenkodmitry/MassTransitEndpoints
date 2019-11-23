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
    public class SmtpInstancesRepository_Specs
    {
        SmtpInstancesRepository _sut;

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

            _sut = new SmtpInstancesRepository();
        }

        [SetUp]
        public void Before_each() => _documentSessionMock.Reset();

        [Test]
        public async Task Should_store_smtp_server()
        {
            //
            const int id = 100;

            var createSmtpInstanceCommand = new CreateSmtpInstanceCommand
            {
                Name = Guid.NewGuid().ToString(),
                InstancesCount = 4,
                SmtpConnectionId = 1000,
                UserCredentialsId = 2000,
            };

            SmtpInstance smtpInstance = null;
            _documentSessionMock
                .Setup(x => x.Insert(IsAny<SmtpInstance>()))
                .Callback<SmtpInstance[]>(instances =>
                {
                    smtpInstance = instances[0];
                    smtpInstance.Id = id;
                });

            //
            await _sut.SendCommand(_contextMock.Object, createSmtpInstanceCommand, CancellationToken.None);

            //
            smtpInstance.Should().BeEquivalentTo(createSmtpInstanceCommand, opts => opts.ExcludingMissingMembers());
        }

        [Test]
        public async Task Should_set_id_on_createdid_payload()
        {
            //
            const int id = 100;

            CreatedId<SmtpInstance, int> createdId = new CreatedId<SmtpInstance, int>();

            _contextMock
                .Setup(x => x.TryGetPayload(out createdId))
                .Returns(true);

            _documentSessionMock
                .Setup(x => x.Insert(IsAny<SmtpInstance>()))
                .Callback<SmtpInstance[]>(instances =>
                {
                    var smtpInstance = instances[0];
                    smtpInstance.Id = id;
                });

            //
            await _sut.SendCommand(_contextMock.Object, new CreateSmtpInstanceCommand(), CancellationToken.None);

            //
            createdId.Id.Should().Be(id);
        }
    }
}
