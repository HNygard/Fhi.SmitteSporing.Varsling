using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Telefoner;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.Telefoner
{
    public class SynkroniserSlettingerMotSimulaTester
    {
        [Fact]
        public async Task Handle_GittIngenTelefoner_GjorIngenting()
        {
            var automocker = new AutoMocker();

            automocker.Setup<ITelefonRespository, Task<List<Telefon>>>(x => x.HentAlleTelefoner())
                .ReturnsAsync(new List<Telefon>());

            var target = automocker.CreateInstance<SynkroniserSlettingerMotSimula.Handler>();

            var antallSlettet = await target.Handle(new SynkroniserSlettingerMotSimula.Command(), new CancellationToken());

            antallSlettet.Should().Be(0);

            automocker.Verify<ITelefonRespository>(x => x.SlettTelefonMedTilknyttetInnhold(It.IsAny<Telefon>()), Times.Never);
        }

        [Fact]
        public async Task Handle_GittIngenTelefonerSlettet_GjorIngenting()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<ITelefonRespository, Task<List<Telefon>>>(x => x.HentAlleTelefoner())
                .ReturnsAsync(new List<Telefon>
                {
                    new Telefon
                    {
                        TelefonId = 42,
                        Telefonnummer = "<kryptert>"
                    }
                });

            automocker
                .Setup<ICryptoManagerFacade, string>(x => x.DekrypterUtenBrukerinnsyn("<kryptert>"))
                .Returns("+4798765432");

            automocker
                .Setup<ISimulaFacade, Task<IEnumerable<string>>>(x =>
                    x.SjekkSlettinger(It.Is<IEnumerable<string>>(x => x.Contains("+4798765432"))))
                .ReturnsAsync(new List<string>());

            var target = automocker.CreateInstance<SynkroniserSlettingerMotSimula.Handler>();

            var antallSlettet = await target.Handle(new SynkroniserSlettingerMotSimula.Command(), new CancellationToken());

            antallSlettet.Should().Be(0);

            automocker.Verify<ITelefonRespository>(x => x.SlettTelefonMedTilknyttetInnhold(It.IsAny<Telefon>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Gitt2av3Slettet_SletterKorrekteTelefoner()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<ITelefonRespository, Task<List<Telefon>>>(x => x.HentAlleTelefoner())
                .ReturnsAsync(new List<Telefon>
                {
                    new Telefon
                    {
                        TelefonId = 41,
                        Telefonnummer = "<kryptert-1>"
                    },
                    new Telefon
                    {
                        TelefonId = 42,
                        Telefonnummer = "<kryptert-2>"
                    },
                    new Telefon
                    {
                        TelefonId = 43,
                        Telefonnummer = "<kryptert-3>"
                    }
                });

            automocker
                .Setup<ICryptoManagerFacade, string>(x => x.DekrypterUtenBrukerinnsyn("<kryptert-1>"))
                .Returns("+4790000000");
            automocker
                .Setup<ICryptoManagerFacade, string>(x => x.DekrypterUtenBrukerinnsyn("<kryptert-2>"))
                .Returns("+4798765432");
            automocker
                .Setup<ICryptoManagerFacade, string>(x => x.DekrypterUtenBrukerinnsyn("<kryptert-3>"))
                .Returns("+4748000000");

            automocker
                .Setup<ISimulaFacade, Task<IEnumerable<string>>>(x =>
                    x.SjekkSlettinger(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new [] { "+4790000000", "+4748000000" });

            var target = automocker.CreateInstance<SynkroniserSlettingerMotSimula.Handler>();

            var antallSlettet = await target.Handle(new SynkroniserSlettingerMotSimula.Command(), new CancellationToken());

            antallSlettet.Should().Be(2);

            automocker.Verify<ITelefonRespository>(
                x => x.SlettTelefonMedTilknyttetInnhold(It.Is<Telefon>(t => t.TelefonId == 41)), Times.Once);

            automocker.Verify<ITelefonRespository>(
                x => x.SlettTelefonMedTilknyttetInnhold(It.Is<Telefon>(t => t.TelefonId == 43)), Times.Once);
        }
    }
}