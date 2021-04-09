using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.Indekspasienter
{
    public class SendVarslerForEldsteGodkjenteTester
    {
        [Fact]
        public async Task Handle_GittIngenGodkjenteIndekspasienter_ReturnererFalse()
        {
            var automocker = new AutoMocker();

            automocker.Use<IEnumerable<IVarslingsregel>>(new List<IVarslingsregel>());

            var target = automocker.CreateInstance<SendVarslerForEldsteGodkjente.Handler>();

            var result = await target.Handle(new SendVarslerForEldsteGodkjente.Command(), new CancellationToken());

            result.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_GittGodkjentPasientIngenKontakter_GirTrueOgOppdatererStatus()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IIndekspasientRepository, Task<Option<Indekspasient>>>(x =>
                    x.HentEldsteGodkjentMenIkkeVarslet())
                .ReturnsAsync(new Indekspasient
                {
                    Varslingsstatus = Varslingsstatus.Godkjent,
                    IndekspasientId = 42
                }.Some());

            automocker.Use<IEnumerable<IVarslingsregel>>(new List<IVarslingsregel>());

            automocker.Setup<ISmittekontaktRespository, Task<List<Smittekontakt>>>(x =>
                    x.HentSmittekontaktTilVarslingForIndekspasient(42))
                .ReturnsAsync(new List<Smittekontakt>());

            var target = automocker.CreateInstance<SendVarslerForEldsteGodkjente.Handler>();

            var result = await target.Handle(new SendVarslerForEldsteGodkjente.Command(), new CancellationToken());

            result.Should().BeTrue();

            automocker.Verify<IIndekspasientRepository>(x => x.OppdaterVarslingsstatus(42, Varslingsstatus.Ferdig));
        }


        [Fact]
        public async Task Handle_GittGodkjentPasientKontakterAvvistAvEnRegel_GirTrueOppdatererStatusMenSenderIngenting()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IIndekspasientRepository, Task<Option<Indekspasient>>>(x =>
                    x.HentEldsteGodkjentMenIkkeVarslet())
                .ReturnsAsync(new Indekspasient
                {
                    Varslingsstatus = Varslingsstatus.Godkjent,
                    IndekspasientId = 42
                }.Some());

            var regelMockAvvis = new Mock<IVarslingsregel>();
            regelMockAvvis
                .Setup(x => x.KanVarsles(It.IsAny<Smittekontakt>()))
                .Returns(false);

            var regelMockOk = new Mock<IVarslingsregel>();
            regelMockOk
                .Setup(x => x.KanVarsles(It.IsAny<Smittekontakt>()))
                .Returns(true);

            automocker.Use<IEnumerable<IVarslingsregel>>(new [] { regelMockAvvis.Object, regelMockOk.Object });

            automocker.Setup<ISmittekontaktRespository, Task<List<Smittekontakt>>>(x =>
                    x.HentSmittekontaktTilVarslingForIndekspasient(42))
                .ReturnsAsync(new List<Smittekontakt> {
                    new Smittekontakt()
                });

            var target = automocker.CreateInstance<SendVarslerForEldsteGodkjente.Handler>();

            var result = await target.Handle(new SendVarslerForEldsteGodkjente.Command(), new CancellationToken());

            result.Should().BeTrue();

            automocker.Verify<IIndekspasientRepository>(x => x.OppdaterVarslingsstatus(42, Varslingsstatus.Ferdig));

            automocker.Verify<ISmsVarselRepository>(x => x.OpprettSmsVarsler(It.IsAny<IEnumerable<SmsVarsel>>()), Times.Never);
            automocker.Verify<ISmsTjenesteFacade>(x => x.OpprettSmsJobb(It.IsAny<int>(), It.IsAny<IEnumerable<SmsUtsending>>()), Times.Never);
        }




        [Fact]
        public async Task Handle_GittGodkjentPasientMedKontakterReglerOk_GirTrueOppdatererStatusOgSenderSmsMedFlettedata()
        {
            var automocker = new AutoMocker();

            var indekspasient = new Indekspasient
            {
                Varslingsstatus = Varslingsstatus.Godkjent,
                IndekspasientId = 42,
                Kommune = new Kommune
                {
                    SmsFletteinfo = "hello!"
                }
            };

            automocker.Setup<IIndekspasientRepository, Task<Option<Indekspasient>>>(x =>
                    x.HentEldsteGodkjentMenIkkeVarslet())
                .ReturnsAsync(indekspasient.Some());

            var regelMockOk = new Mock<IVarslingsregel>();
            regelMockOk
                .Setup(x => x.KanVarsles(It.IsAny<Smittekontakt>()))
                .Returns(true);

            automocker.Setup<IApplikasjonsinnstillingRepository, Task<Option<int>>>(x =>
                    x.HentInnstilling<int>(It.IsAny<string>()))
                .ReturnsAsync(42.Some());

            automocker.Setup<ISmsTjenesteFacade, Task<int>>(x =>
                    x.OpprettSmsJobb(42, It.IsAny<IEnumerable<SmsUtsending>>()))
                .ReturnsAsync(123);

            automocker.Setup<ICryptoManagerFacade, string>(x => x.DekrypterUtenBrukerinnsyn("<kryptert-tlf>"))
                .Returns("+4798765432");

            automocker.Use<IEnumerable<IVarslingsregel>>(new[] { regelMockOk.Object });

            automocker.Setup<ISmittekontaktRespository, Task<List<Smittekontakt>>>(x =>
                    x.HentSmittekontaktTilVarslingForIndekspasient(42))
                .ReturnsAsync(new List<Smittekontakt> {
                    new Smittekontakt
                    {
                        Telefon = new Telefon
                        {
                            Telefonnummer = "<kryptert-tlf>"
                        },
                        Indekspasient = indekspasient,
                        Detaljer = new List<SmittekontaktDetaljer>
                        {
                            new SmittekontaktDetaljer
                            {
                                Dato = DateTime.Now
                            }
                        }
                    }
                });

            var target = automocker.CreateInstance<SendVarslerForEldsteGodkjente.Handler>();

            var result = await target.Handle(new SendVarslerForEldsteGodkjente.Command(), new CancellationToken());

            result.Should().BeTrue();

            automocker.Verify<IIndekspasientRepository>(x => x.OppdaterVarslingsstatus(42, Varslingsstatus.Ferdig));

            automocker.Verify<ISmsVarselRepository>(x => x.OpprettSmsVarsler(It.IsAny<IEnumerable<SmsVarsel>>()), Times.Once);
            automocker.Verify<ISmsTjenesteFacade>(x => x.OpprettSmsJobb(42, It.Is<IEnumerable<SmsUtsending>>(utsendinger =>
                utsendinger.Any(u => u.Telefonnummer == "+4798765432" && 
                    new []
                    {
                        "kontaktDato", "kommuneInfo", "risikoTekst", "verifiseringskode"
                    }.All(f => u.Flettedata.ContainsKey(f))))
            ), Times.Once);
            automocker.Verify<ISmsTjenesteFacade>(x => x.StartSmsJobb(123), Times.Once);
        }

    }
}