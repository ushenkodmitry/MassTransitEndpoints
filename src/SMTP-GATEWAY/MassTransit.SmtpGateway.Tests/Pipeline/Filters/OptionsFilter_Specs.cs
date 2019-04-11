using GreenPipes;
using MassTransit.SmtpGateway.Contexts;
using MassTransit.SmtpGateway.Options;
using MassTransit.SmtpGateway.Pipeline.Filters;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using static Moq.Mock;
using static Moq.It;
using static Moq.Times;
using FluentAssertions;

namespace MassTransit.SmtpGateway.Pipeline.Filters
{
    [Category("SmtpGateway")]
    [TestFixture]
    public class OptionsFilter_Specs
    {
        IFilter<PipeContext> _sut;

        ServerOptions _serverOptions;

        BehaviorOptions _behaviorOptions;

        [OneTimeSetUp]
        public void A_filter_being_tested()
        {
            _serverOptions = new ServerOptions();
            _behaviorOptions = new BehaviorOptions();

            _sut = new OptionsFilter<PipeContext>(_serverOptions, _behaviorOptions);
        }

        [Test]
        public async Task Should_add_options_context_to_payloads_on_send()
        {
            //
            var pipeContextMock = new Mock<PipeContext>();

            PayloadFactory<OptionsContext> payloadFactory = null;
            pipeContextMock
                .Setup(x => x.GetOrAddPayload(IsAny<PayloadFactory<OptionsContext>>()))
                .Callback<PayloadFactory<OptionsContext>>(_ => payloadFactory = _);

            //
            await _sut.Send(pipeContextMock.Object, Of<IPipe<PipeContext>>());

            //
            var optionsContext = payloadFactory();
            optionsContext.BehaviorOptions.Should().BeSameAs(_behaviorOptions);
            optionsContext.ServerOptions.Should().BeSameAs(_serverOptions);
        }
    }
}
