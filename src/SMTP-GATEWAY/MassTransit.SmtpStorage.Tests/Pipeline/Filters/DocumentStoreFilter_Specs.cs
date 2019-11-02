using System.Threading.Tasks;
using FluentAssertions;
using GreenPipes;
using MassTransit.Contexts;
using Moq;
using NUnit.Framework;
using static Moq.It;
using static Moq.Mock;
using static Moq.Times;

namespace MassTransit.Pipeline.Filters
{
    [TestFixture]
    [Category("SmtpStorage, Filters")]
    public class DocumentStoreFilter_Specs
    {
        IFilter<PipeContext> _sut;

        Mock<IDocumentStoreFactory> _documentStoreFactoryMock;

        PayloadFactory<DocumentStoreContext> _payloadFactory;

        Mock<IPipe<PipeContext>> _nextMock;

        Mock<PipeContext> _contextMock;

        [OneTimeSetUp]
        public async Task A_filter_being_tested()
        {
            _documentStoreFactoryMock = new Mock<IDocumentStoreFactory>();

            _sut = new DocumentStoreFilter<PipeContext>(
                options => { },
                _documentStoreFactoryMock.Object);

            _contextMock = new Mock<PipeContext>();
            _contextMock
                .Setup(x => x.GetOrAddPayload(IsAny<PayloadFactory<DocumentStoreContext>>()))
                .Callback<PayloadFactory<DocumentStoreContext>>(factory => _payloadFactory = factory)
                .Returns(Of<DocumentStoreContext>());

            _nextMock = new Mock<IPipe<PipeContext>>();

            await _sut.Send(_contextMock.Object, _nextMock.Object);
        }

        [Test]
        public void Should_add_document_store_context_as_payload()
        {
            //
            var context = _payloadFactory();
            context.Should().NotBeNull();
        }

        [Test]
        public void Should_call_next_once()
        {
            //
            _nextMock
                .Verify(x => x.Send(_contextMock.Object), Once);
        }
    }
}
