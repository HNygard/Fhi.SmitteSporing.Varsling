using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Konstanter;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Smittesporing.Varsling.Domene.SmsTestmeldinger;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using Moq;
using Moq.AutoMock;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.SmsTestmeldinger
{
    public class LagTestmeldingTester
    {
        [Fact]
        public async Task LagTestmelding_GittAlleInputdata_LagerTestemeldingMedRiktigFlettefelter()
        {
            //arrange
            var automocker = new AutoMocker();

            automocker.Setup<IApplikasjonsinnstillingRepository, Task<Option<int>>>(x => x.HentInnstilling<int>(Applikasjonsinnstillinger.SmsVarselMalId))
                .ReturnsAsync(42.Some());

            automocker.Setup<IApplikasjonsinnstillingRepository, Task<Option<SmsFletteinnstillinger>>>(x => x.HentInnstilling<SmsFletteinnstillinger>(Applikasjonsinnstillinger.SmsFletteinnstillinger))
                .ReturnsAsync(new SmsFletteinnstillinger
                {
                    LavRisikokategori = "Du hadde lav risiko."
                }.Some());

            automocker.Setup<IKommuneRepository, Task<Option<Kommune>>>(x => x.HentForId(12))
                .ReturnsAsync(new Kommune
                {
                    KommuneId = 12,
                    SmsFletteinfo = "Velkommen til Korona herad."
                }.Some());

            automocker.Setup<ICryptoManagerFacade, string>(
                    x => x.KrypterUtenBrukerinnsyn("98765432"))
                .Returns("<kryptert>");
            automocker.Setup<ICryptoManagerFacade, string>(
                    x => x.DekrypterUtenBrukerinnsyn("<kryptert>"))
                .Returns("98765432");

            var target = automocker.CreateInstance<LagTestmelding.Handler>();

            var request = new LagTestmelding.Command
            {
                Testmelding = new SmsTestmeldingAm
                {
                    Telefonnummer = "98765432",
                    KommuneId = 12,
                    Risikokategori = "low",
                    Referanse = Guid.Parse("2144194b-2c4f-4bcf-b055-d5db5cf46254"),
                    DatoSisteKontakt = DateTime.Parse("2020-05-01T12:00:00.000Z")
                }
            };

            //act
            await target.Handle(request, new CancellationToken());

            //assert
            automocker.Verify<ISmsTjenesteFacade>(x => x.SendTestmeldingForMal(42, It.Is<SmsUtsending>(u =>
                u.Referanse == Guid.Parse("2144194b-2c4f-4bcf-b055-d5db5cf46254") &&
                u.Telefonnummer == "98765432" &&
                u.Flettedata["kommuneInfo"] == "Velkommen til Korona herad." &&
                u.Flettedata["risikoTekst"] == "Du hadde lav risiko." &&
                u.Flettedata["kontaktDato"] == "1. mai 2020")));
        }
    }
}