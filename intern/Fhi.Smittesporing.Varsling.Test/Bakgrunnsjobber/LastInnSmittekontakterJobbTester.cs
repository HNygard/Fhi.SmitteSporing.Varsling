using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Bakgrunnsjobber
{
    public class LastInnSmittekontakterJobbTester
    {
        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittKonfig_StarterLastInnSmittekontakterCommandMedRiktiParams()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IOptions<LastInnSmittekontakterJobb.Konfig>, LastInnSmittekontakterJobb.Konfig>(x => x.Value)
                .Returns(new LastInnSmittekontakterJobb.Konfig
                {
                    MaksAntallDagerBakover = 42,
                    AntallDagerForProvedato = 13
                });

            var target = automocker.CreateInstance<LastInnSmittekontakterJobb>();

            await target.UtforJobb(new CancellationToken());

            automocker.Verify<IMediator>(
                m => m.Send(
                    It.Is<LastInnSmittekontakter.Command>(c => 
                        c.AntallDagerForProvedato == 13 &&
                        c.MaksAntallDagerBakover == 42),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittKontakterOpprettet_ReturnererTrue()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IOptions<LastInnSmittekontakterJobb.Konfig>, LastInnSmittekontakterJobb.Konfig>(x => x.Value)
                .Returns(new LastInnSmittekontakterJobb.Konfig());

            automocker
                .Setup<IMediator, Task<int>>(x => x.Send(It.IsAny<LastInnSmittekontakter.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(42);

            var target = automocker.CreateInstance<LastInnSmittekontakterJobb>();

            var result = await target.UtforJobb(new CancellationToken());

            result.Should().BeTrue();
        }


        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittIngenKontakterOpprettet_ReturnererFalse()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IOptions<LastInnSmittekontakterJobb.Konfig>, LastInnSmittekontakterJobb.Konfig>(x => x.Value)
                .Returns(new LastInnSmittekontakterJobb.Konfig());

            automocker
                .Setup<IMediator, Task<int>>(x => x.Send(It.IsAny<LastInnSmittekontakter.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var target = automocker.CreateInstance<LastInnSmittekontakterJobb>();

            var result = await target.UtforJobb(new CancellationToken());

            result.Should().BeFalse();
        }
    }
}