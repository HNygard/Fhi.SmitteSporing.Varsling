using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Domene.Konstanter;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Msis;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Bakgrunnsjobber
{
    public class HenteNyeIndekspasienterJobbTester
    {
        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittNyeTilfeller_ForsokerOppretteAlle()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IOptions<HenteNyeIndekspasienterJobb.Konfig>, HenteNyeIndekspasienterJobb.Konfig>(x => x.Value)
                .Returns(new HenteNyeIndekspasienterJobb.Konfig
                {
                    MaksTidBakover = TimeSpan.FromDays(2)
                });

            automocker
                .Setup<IMediator, Task<IEnumerable<MsisSmittetilfelle>>>(m => m.Send(It.IsAny<HentFraMsis.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HentFraMsis.Query q, CancellationToken _) => new List<MsisSmittetilfelle>
                {
                    new MsisSmittetilfelle
                    {
                        Fodselsnummer = null,
                        Opprettettidspunkt=q.FraDato.AddDays(1)
                    },
                    new MsisSmittetilfelle
                    {
                        Fodselsnummer = null,
                        Opprettettidspunkt=q.FraDato.AddDays(2)
                    }
                });

            var target = automocker.CreateInstance<HenteNyeIndekspasienterJobb>();

            var nyeBehandlet = await target.UtforJobb(new CancellationToken());

            Assert.True(nyeBehandlet);

            automocker.Verify<IMediator>(
                m => m.Send(It.IsAny<ForsokOpprett.Command>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittNyttTilfeller_OppdatererTidOgHashForSync()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IOptions<HenteNyeIndekspasienterJobb.Konfig>, HenteNyeIndekspasienterJobb.Konfig>(x => x.Value)
                .Returns(new HenteNyeIndekspasienterJobb.Konfig
                {
                    MaksTidBakover = TimeSpan.FromDays(2)
                });

            automocker
                .Setup<IMediator, Task<IEnumerable<MsisSmittetilfelle>>>(m => m.Send(It.IsAny<HentFraMsis.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HentFraMsis.Query q, CancellationToken _) => new List<MsisSmittetilfelle>
                {
                    new MsisSmittetilfelle
                    {
                        Fodselsnummer = "23017812345",
                        Opprettettidspunkt = DateTime.Now.AddHours(-1),
                        Provedato = DateTime.Now.AddDays(-1)
                    },
                    new MsisSmittetilfelle
                    {
                        Fodselsnummer = "23017812345",
                        Opprettettidspunkt = DateTime.Now.AddHours(-1).AddMinutes(1),
                        Provedato = DateTime.Now.AddDays(-1).AddMinutes(1)
                    }
                });

            var target = automocker.CreateInstance<HenteNyeIndekspasienterJobb>();

            var nyeBehandlet = await target.UtforJobb(new CancellationToken());

            Assert.True(nyeBehandlet);

            automocker.Verify<IApplikasjonsinnstillingRepository>(
                r => r.SettInnstilling(Applikasjonsinnstillinger.SisteBehandletMsisHash, It.IsAny<string>()),
                Times.Exactly(2));
            automocker.Verify<IApplikasjonsinnstillingRepository>(
                r => r.SettInnstilling(Applikasjonsinnstillinger.SisteBehandletMsisTidspunkt, It.IsAny<DateTime>()),
                Times.Exactly(2));
        }

        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittGamleTilfelle_BehandlerMenOppretterIngenting()
        {
            var forrigeSyncTidspunkt = DateTime.Now.AddHours(-1);

            var automocker = new AutoMocker();
            automocker
                .Setup<IOptions<HenteNyeIndekspasienterJobb.Konfig>, HenteNyeIndekspasienterJobb.Konfig>(x => x.Value)
                .Returns(new HenteNyeIndekspasienterJobb.Konfig
                {
                    MaksTidBakover = TimeSpan.FromDays(2)
                });
            automocker
                .Setup<IApplikasjonsinnstillingRepository, Task<Option<DateTime>>>(x =>
                    x.HentInnstilling<DateTime>(Applikasjonsinnstillinger.SisteBehandletMsisTidspunkt))
                .ReturnsAsync((string _) => forrigeSyncTidspunkt.AddSeconds(-1).Some());
            automocker
                .Setup<IMediator, Task<IEnumerable<MsisSmittetilfelle>>>(m => m.Send(It.IsAny<HentFraMsis.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HentFraMsis.Query q, CancellationToken _) => new List<MsisSmittetilfelle>
                {
                    new MsisSmittetilfelle
                    {
                        Fodselsnummer = "23017812345",
                        Opprettettidspunkt = forrigeSyncTidspunkt.AddHours(-1),
                        Provedato = forrigeSyncTidspunkt.AddDays(-1)
                    },
                    new MsisSmittetilfelle
                    {
                        Fodselsnummer = "23017812345",
                        Opprettettidspunkt = forrigeSyncTidspunkt.AddHours(-1).AddMinutes(1),
                        Provedato = forrigeSyncTidspunkt.AddDays(-1).AddMinutes(1)
                    }
                });

            var target = automocker.CreateInstance<HenteNyeIndekspasienterJobb>();

            var nyeBehandlet = await target.UtforJobb(new CancellationToken());

            Assert.False(nyeBehandlet);

            automocker.Verify<IMediator>(
                m => m.Send(It.IsAny<ForsokOpprett.Command>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittForrigeTilfelleIgjen_OkerSyncstatusTilNesteMillisekundForAGaVidere()
        {
            var forrigeTilfelle = new MsisSmittetilfelle
            {
                Fodselsnummer = "23017812345",
                Opprettettidspunkt = DateTime.Now.AddHours(-1).AddMinutes(1),
                Provedato = DateTime.Now.AddDays(-1).AddMinutes(1)
            };

            var automocker = new AutoMocker();
            automocker
                .Setup<IOptions<HenteNyeIndekspasienterJobb.Konfig>, HenteNyeIndekspasienterJobb.Konfig>(x => x.Value)
                .Returns(new HenteNyeIndekspasienterJobb.Konfig
                {
                    MaksTidBakover = TimeSpan.FromDays(2)
                });
            automocker
                .Setup<IApplikasjonsinnstillingRepository, Task<Option<DateTime>>>(x =>
                    x.HentInnstilling<DateTime>(Applikasjonsinnstillinger.SisteBehandletMsisTidspunkt))
                .ReturnsAsync(forrigeTilfelle.Opprettettidspunkt.Some());
            automocker
                .Setup<IApplikasjonsinnstillingRepository, Task<Option<string>>>(x =>
                    x.HentInnstilling<string>(Applikasjonsinnstillinger.SisteBehandletMsisHash))
                .ReturnsAsync(HenteNyeIndekspasienterJobb.HashMsisTilfelle(forrigeTilfelle).Some());
            automocker
                .Setup<IMediator, Task<IEnumerable<MsisSmittetilfelle>>>(m => m.Send(It.IsAny<HentFraMsis.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HentFraMsis.Query q, CancellationToken _) => new List<MsisSmittetilfelle>
                {
                    forrigeTilfelle
                });

            var target = automocker.CreateInstance<HenteNyeIndekspasienterJobb>();

            var nyeBehandlet = await target.UtforJobb(new CancellationToken());

            Assert.False(nyeBehandlet);

            automocker.Verify<IApplikasjonsinnstillingRepository>(
                x => x.SettInnstilling(Applikasjonsinnstillinger.SisteBehandletMsisTidspunkt, forrigeTilfelle.Opprettettidspunkt.AddMilliseconds(1)),
                Times.Once);

            automocker.Verify<IApplikasjonsinnstillingRepository>(
                x => x.SettInnstilling(Applikasjonsinnstillinger.SisteBehandletMsisHash, string.Empty),
                Times.Once);

            automocker.Verify<IMediator>(
                m => m.Send(It.IsAny<ForsokOpprett.Command>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittTomListe_GjorIngenting()
        {
            var automocker = new AutoMocker();
            automocker
                .Setup<IOptions<HenteNyeIndekspasienterJobb.Konfig>, HenteNyeIndekspasienterJobb.Konfig>(x => x.Value)
                .Returns(new HenteNyeIndekspasienterJobb.Konfig
                {
                    MaksTidBakover = TimeSpan.FromDays(2)
                });
            automocker
                .Setup<IMediator, Task<IEnumerable<MsisSmittetilfelle>>>(m => m.Send(It.IsAny<HentFraMsis.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HentFraMsis.Query q, CancellationToken _) => new List<MsisSmittetilfelle>());

            var target = automocker.CreateInstance<HenteNyeIndekspasienterJobb>();

            var nyeBehandlet = await target.UtforJobb(new CancellationToken());

            Assert.False(nyeBehandlet);

            automocker.Verify<IApplikasjonsinnstillingRepository>(
                r => r.SettInnstilling(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            automocker.Verify<IApplikasjonsinnstillingRepository>(
                r => r.SettInnstilling(It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);
            automocker.Verify<IMediator>(
                m => m.Send(It.IsAny<ForsokOpprett.Command>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittDuplikateSammenMedNytt_OppretterKunNytt()
        {
            var forrigeSyncTidspunkt = DateTime.Now.AddDays(-1);
            var forrigeTilfelle = new MsisSmittetilfelle
            {
                Opprettettidspunkt = forrigeSyncTidspunkt,
                Provedato = forrigeSyncTidspunkt.AddDays(-1),
                Fodselsnummer = "23036712345"
            };
            var forrigeSyncHash = HenteNyeIndekspasienterJobb.HashMsisTilfelle(forrigeTilfelle);
            var automocker = new AutoMocker();
            automocker
                .Setup<IOptions<HenteNyeIndekspasienterJobb.Konfig>, HenteNyeIndekspasienterJobb.Konfig>(x => x.Value)
                .Returns(new HenteNyeIndekspasienterJobb.Konfig
                {
                    MaksTidBakover = TimeSpan.FromDays(2)
                });
            automocker
                .Setup<IApplikasjonsinnstillingRepository, Task<Option<DateTime>>>(x =>
                    x.HentInnstilling<DateTime>(Applikasjonsinnstillinger.SisteBehandletMsisTidspunkt))
                .ReturnsAsync((string _) => forrigeSyncTidspunkt.Some());
            automocker
                .Setup<IApplikasjonsinnstillingRepository, Task<Option<string>>>(x =>
                    x.HentInnstilling<string>(Applikasjonsinnstillinger.SisteBehandletMsisHash))
                .ReturnsAsync((string _) => forrigeSyncHash.Some());
            automocker
                .Setup<IMediator, Task<IEnumerable<MsisSmittetilfelle>>>(m => m.Send(It.IsAny<HentFraMsis.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HentFraMsis.Query q, CancellationToken _) => new List<MsisSmittetilfelle>
                {
                    // Eldre tilfelle (eldre tidspunkt)
                    new MsisSmittetilfelle
                    {
                        Fodselsnummer = "23017812345",
                        Opprettettidspunkt = forrigeSyncTidspunkt.AddSeconds(-1),
                        Provedato = forrigeSyncTidspunkt.AddDays(-1)
                    },
                    // Eldre tilfelle (samme tidspunkt)
                    new MsisSmittetilfelle
                    {
                        Fodselsnummer = "32123812345",
                        Opprettettidspunkt = forrigeSyncTidspunkt,
                        Provedato = forrigeSyncTidspunkt.AddDays(-1)
                    },
                    forrigeTilfelle,
                    // Nytt tilfelle
                    new MsisSmittetilfelle
                    {
                        Fodselsnummer = "23017812345",
                        Opprettettidspunkt = forrigeSyncTidspunkt.AddSeconds(1),
                        Provedato = forrigeSyncTidspunkt.AddDays(-1)
                    }
                });

            var target = automocker.CreateInstance<HenteNyeIndekspasienterJobb>();

            var nyeBehandlet = await target.UtforJobb(new CancellationToken());

            Assert.True(nyeBehandlet);

            automocker.Verify<IApplikasjonsinnstillingRepository>(
                r => r.SettInnstilling(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
            automocker.Verify<IApplikasjonsinnstillingRepository>(
                r => r.SettInnstilling(It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Once);
            automocker.Verify<IMediator>(
                m => m.Send(It.Is<ForsokOpprett.Command>(c => c.Fodselsnummer == "23017812345"), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        [Trait("HostService", "HenteNyeSmittetilfeller")]
        public async Task UtforJobb_GittNyttTilfelle_ForsokerOpprettingMedAlleData()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IOptions<HenteNyeIndekspasienterJobb.Konfig>, HenteNyeIndekspasienterJobb.Konfig>(x => x.Value)
                .Returns(new HenteNyeIndekspasienterJobb.Konfig
                {
                    MaksTidBakover = TimeSpan.FromDays(2)
                });

            var nyttTilfelle = new MsisSmittetilfelle
            {
                Fodselsnummer = "270490765432",
                Opprettettidspunkt = DateTime.Now.AddDays(1),
                Provedato = DateTime.Now.AddDays(-2),
                Bostedkommune = "Testkommunen",
                Bostedkommunenummer = "7357"
            };

            automocker
                .Setup<IMediator, Task<IEnumerable<MsisSmittetilfelle>>>(m => m.Send(It.IsAny<HentFraMsis.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HentFraMsis.Query q, CancellationToken _) => new List<MsisSmittetilfelle>
                {
                    nyttTilfelle
                });

            var target = automocker.CreateInstance<HenteNyeIndekspasienterJobb>();

            var nyeBehandlet = await target.UtforJobb(new CancellationToken());

            Assert.True(nyeBehandlet);

            automocker.Verify<IMediator>(
                m => m.Send(It.Is<ForsokOpprett.Command>(c => 
                    c.Fodselsnummer == nyttTilfelle.Fodselsnummer &&
                    c.Opprettettidspunkt == nyttTilfelle.Opprettettidspunkt &&
                    c.Provedato == nyttTilfelle.Provedato &&
                    c.Bostedkommune == nyttTilfelle.Bostedkommune &&
                    c.Bostedkommunenummer == nyttTilfelle.Bostedkommunenummer
                ), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}