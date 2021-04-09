using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Datalag;
using Fhi.Smittesporing.Varsling.Datalag.Repositories;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.Indekspasienter
{
    public class LastInnSmittekontakterTester : DomeneTestBase
    {
        private readonly ISmittekontaktRespository _smittekontaktRespository;
        private readonly ITelefonRespository _telefonRespository;
        private readonly Mock<ISimulaFacade> _simulaFacade;
        private readonly Mock<ICryptoManagerFacade> _cryptoFacade;
        private readonly Mock<ILogger<LastInnSmittekontakter.Handler>> _logger;

        public LastInnSmittekontakterTester()
        {
            _smittekontaktRespository = new SmittekontaktRespository(DbContext);
            _telefonRespository = new TelefonRepository(DbContext);
            _simulaFacade = new Mock<ISimulaFacade>();
            _cryptoFacade = new Mock<ICryptoManagerFacade>();
            _logger = new Mock<ILogger<LastInnSmittekontakter.Handler>>();

            _cryptoFacade.Setup(m => m.KrypterUtenBrukerinnsyn(It.IsAny<string>())).Returns((string s) => "krypt:" + s);
            _cryptoFacade.Setup(m => m.DekrypterUtenBrukerinnsyn(It.IsAny<string>())).Returns((string s) => s.Replace("krypt:", ""));
        }

        [Fact]
        [Trait("Indekspasient", "LastInnSmittekontakter")]
        public async Task Handle_Gitt2KontakterAllePasienter_SetterSmitteKontaktOgLagerKontakter()
        {
            DbContext.Indekspasienter.AddRange(Enumerable.Range(0, 3).Select(_ => new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = DateTime.Now.AddDays(-3),
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Telefon = new Telefon
                {
                    Telefonnummer = "krypt:" + RandomTelefonNummer()
                }
            }));
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((string tlf, DateTime fra, DateTime til) => LagSimulaKontaktrapport(tlf, RandomTelefonNummer(), RandomTelefonNummer()).Some());

            //arrange
            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command();

            //act
            var indekspasienterKontaktsjekket = await handler.Handle(request, CancellationToken);

            //assert
            Assert.Equal(3, indekspasienterKontaktsjekket); // 3 indekspasienter i mockdata
            DbContext.Indekspasienter.All(i => i.Status == IndekspasientStatus.SmitteKontakt).Should().BeTrue();
            DbContext.Smittekontakt.Count().Should().Be(6); // 2 kontakter per pasient
        }

        [Fact]
        [Trait("Indekspasient", "LastInnSmittekontakter")]
        public async Task Handle_GittKontaktForPasient_LagrerKontaktMedData()
        {
            DbContext.Indekspasienter.Add(new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = DateTime.Now.AddDays(-3),
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Telefon = new Telefon
                {
                    Telefonnummer = "krypt:" + RandomTelefonNummer()
                }
            });
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((string tlf, DateTime fra, DateTime til) => LagSimulaKontaktrapport(tlf, RandomTelefonNummer()).Some());

            //arrange
            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command();

            //act
            await handler.Handle(request, CancellationToken);

            //assert
            var opprettetKontakt = DbContext.Smittekontakt.First();
            opprettetKontakt.Should().NotBeNull();
            opprettetKontakt.Telefon.Should().NotBeNull();
            opprettetKontakt.Verifiseringskode.Should().Be("test123");
        }


        [Fact]
        [Trait("Indekspasient", "LastInnSmittekontakter")]
        public async Task Handle_Gitt0KontakterAllePasienter_SetterIkkeSmitteKontakt()
        {
            DbContext.Indekspasienter.AddRange(Enumerable.Range(0, 3).Select(_ => new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = DateTime.Now.AddDays(-3),
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Telefon = new Telefon
                {
                    Telefonnummer = "krypt:" + RandomTelefonNummer()
                }
            }));
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((string tlf, DateTime fra, DateTime til) => LagSimulaKontaktrapport(tlf).Some());

            //arrange
            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command();

            //act
            var indekspasienterKontaktsjekket = await handler.Handle(request, CancellationToken);

            //assert
            Assert.Equal(3, indekspasienterKontaktsjekket); // 3 indekspasienter i mockdata
            DbContext.Indekspasienter.All(i => i.Status == IndekspasientStatus.IkkeSmitteKontakt).Should().BeTrue();
            DbContext.Smittekontakt.Count().Should().Be(0); // 2 kontakter per pasient
        }

        [Fact]
        [Trait("Indekspasient", "LastInnSmittekontakter")]
        public async Task Handle_GittNoneRapportFraSimula_SletterPasientdata()
        {
            DbContext.Indekspasienter.AddRange(Enumerable.Range(0, 3).Select(_ => new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = DateTime.Now.AddDays(-3),
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Telefon = new Telefon
                {
                    Telefonnummer = "krypt:" + RandomTelefonNummer()
                }
            }));
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(Option.None<SimulaKontaktrapport>());

            //arrange
            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command();

            //act

            var totalAntall = await handler.Handle(request, CancellationToken);

            //assert
            Assert.Equal(3, totalAntall); // 3 indekspasienter i mockdata
            DbContext.Indekspasienter.All(i => i.Status == IndekspasientStatus.Slettet).Should().BeTrue();
        }

        [Fact]
        [Trait("Indekspasient", "LastInnSmittekontakter")]
        public async Task TestCase_SimulaFacadeGetSmittekontakterGirUnntak()
        {
            DbContext.Indekspasienter.Add(new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = DateTime.Now.AddDays(-3),
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Telefon = new Telefon
                {
                    Telefonnummer = "krypt:" + RandomTelefonNummer()
                }
            });
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ThrowsAsync(new Exception("test"));

            //arrange
            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object,_telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command();

            //act
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(request, CancellationToken));
        }

        [Fact]
        [Trait("Indekspasient", "LastInnSmittekontakter")]
        public async Task TestCase_UtenTelefon()
        {
            //indekspasient uten telefon
            DbContext.Indekspasienter.Add(new Indekspasient
            {
                Telefon = null,
                TelefonId = null
            });
            DbContext.SaveChanges();

            //arrange
            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command();

            var antallHentetFor = await handler.Handle(request, CancellationToken);

            Assert.Equal(0, antallHentetFor);
        }

        /// <summary>
        /// For smittetilfeller, der det blir returnert Smiitekontakter fra Simula 
        ///         dersom smittekontakt-telefonnummer finnes, skal den gjennombrukkes, ellers opprettes ny telefon-entity
        ///       return true
        /// </summary>
        [Fact]
        [Trait("HostService", "LastInnSmittekontakter")]
        public async Task Handle_GittTelefonForKontaktErNy_OppretterTelefon()
        {
            string smittertelefon = "krypt:+4798765432";
            string kontakttelefon = "+4747898765";

            DbContext.Indekspasienter.Add(new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = DateTime.Now.AddDays(-3),
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Fodselsnummer = "krypt:12345678901",
                Telefon = new Telefon
                {
                    Telefonnummer = smittertelefon
                }
            });
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter("+4798765432", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((string tlf, DateTime fra, DateTime til) => LagSimulaKontaktrapport(tlf, kontakttelefon).Some());

            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command();

            //act
            var antallHentetFor = await handler.Handle(request, CancellationToken);

            antallHentetFor.Should().Be(1);
            DbContext.Indekspasienter
                .Should().Contain(i => i.Status == IndekspasientStatus.SmitteKontakt && i.Fodselsnummer == "krypt:12345678901");
            DbContext.Smittekontakt
                .Should().Contain(k => k.Telefon.Telefonnummer == "krypt:+4747898765");
            DbContext.Telefon.Should().Contain(t => t.Telefonnummer == "krypt:+4747898765");
        }

        /// <summary>
        /// For smittetilfeller, der det blir returnert Smiitekontakter fra Simula 
        ///         dersom smittekontakt-telefonnummer finnes, skal den gjennombrukkes, ellers opprettes ny telefon-entity
        ///       return true
        /// </summary>
        [Fact]
        public async Task Handle_GittTelefonForKontaktFinnesFraFor_GjenbrukerTelefon()
        {
            string smittertelefon = "krypt:+4798765432";
            string kontakttelefon = "+4747898765";

            DbContext.Indekspasienter.AddRange(new Indekspasient
            {
                Status = IndekspasientStatus.IkkeSmitteKontakt,
                Provedato = DateTime.Now.AddDays(-4),
                Opprettettidspunkt = DateTime.Now.AddDays(-2),
                Fodselsnummer = "krypt:22029012312",
                Telefon = new Telefon
                {
                    Telefonnummer = kontakttelefon
                }
            }, new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = DateTime.Now.AddDays(-3),
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Fodselsnummer = "krypt:12345678901",
                Telefon = new Telefon
                {
                    Telefonnummer = smittertelefon
                }
            });
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter("+4798765432", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((string tlf, DateTime fra, DateTime til) => LagSimulaKontaktrapport(tlf, kontakttelefon).Some());

            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command();

            //act
            var antallHentetFor = await handler.Handle(request, CancellationToken);

            antallHentetFor.Should().Be(1);
            DbContext.Indekspasienter
                .Should().Contain(i => i.Status == IndekspasientStatus.SmitteKontakt && i.Fodselsnummer == "krypt:12345678901");
            DbContext.Smittekontakt
                .Should().Contain(k => k.Telefon.Telefonnummer == "krypt:" + kontakttelefon);
            DbContext.Telefon.Count(t => t.Telefonnummer == "krypt:" + kontakttelefon).Should().Be(1);
        }

        [Fact]
        public async Task Handle_GittTelefonFlereGangerSammeRapport_LagrerBareEnNyTelefon()
        {
            string kontakttelefon1 = "+4747898765";
            string kontakttelefon2 = "004747898765";

            DbContext.Indekspasienter.AddRange(new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = DateTime.Now.AddDays(-4),
                Opprettettidspunkt = DateTime.Now.AddDays(-2),
                Fodselsnummer = "krypt:22029012312",
                Telefon = new Telefon
                {
                    Telefonnummer = "krypt:+4798765432"
                }
            });
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter("+4798765432", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((string tlf, DateTime fra, DateTime til) => LagSimulaKontaktrapport(tlf, kontakttelefon1, kontakttelefon2).Some());

            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command();

            //act
            var antallHentetFor = await handler.Handle(request, CancellationToken);

            antallHentetFor.Should().Be(1);
            // To nye smittekontakter sammme TLF
            DbContext.Smittekontakt
                .Count(k => k.Telefon.Telefonnummer == "krypt:+4747898765").Should().Be(2);
            // Totalt 2 TLF: indekspasient + felles TLF kontakter
            DbContext.Telefon.Count().Should().Be(2);
        }


        [Theory]
        [Trait("Indekspasient", "LastInnSmittekontakter")]
        [InlineData(1, 8, 0)]
        [InlineData(7, 14, 6)]
        [InlineData(13, 14, 12)]
        public async Task Handle_GittProvedato_HenterKontakterRiktigRange(int dagerSidenProvedato, int forventetDagerSidenStart, int forventetDagerSidenSlutt)
        {
            DbContext.Indekspasienter.Add(new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = DateTime.Now.AddDays(-dagerSidenProvedato),
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Telefon = new Telefon
                {
                    Telefonnummer = "krypt:" + RandomTelefonNummer()
                }
            });
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((string tlf, DateTime fra, DateTime til) => LagSimulaKontaktrapport(tlf).Some());

            //arrange
            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command
            {
                MaksAntallDagerBakover = 14,
                AntallDagerForProvedato = 7
            };

            //act
            await handler.Handle(request, CancellationToken);

            //assert
            _simulaFacade.Verify(x => x.GetSmittekontakter(
                It.IsAny<string>(), 
                It.Is<DateTime>(d => d == DateTime.Today.AddDays(-forventetDagerSidenStart)),
                It.Is<DateTime>(d => d == DateTime.Today.AddDays(-forventetDagerSidenSlutt))
            ), Times.Once());
        }

        [Fact]
        [Trait("Indekspasient", "LastInnSmittekontakter")]
        public async Task Handle_GittUgyldigProvedato_SletterPasient()
        {
            DbContext.Indekspasienter.Add(new Indekspasient
            {
                Status = IndekspasientStatus.Registrert,
                Provedato = default,
                Opprettettidspunkt = DateTime.Now.AddDays(-1),
                Telefon = new Telefon
                {
                    Telefonnummer = "krypt:" + RandomTelefonNummer()
                }
            });
            DbContext.SaveChanges();

            _simulaFacade
                .Setup(m => m.GetSmittekontakter(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((string tlf, DateTime fra, DateTime til) => LagSimulaKontaktrapport(tlf).Some());

            //arrange
            var handler = new LastInnSmittekontakter.Handler(IndekspasientRepository, _smittekontaktRespository, _simulaFacade.Object, _telefonRespository, _cryptoFacade.Object, new TelefonNormalFacade(), _logger.Object);
            var request = new LastInnSmittekontakter.Command
            {
                MaksAntallDagerBakover = 14,
                AntallDagerForProvedato = 7
            };

            //act
            await handler.Handle(request, CancellationToken);

            //assert
            DbContext.Indekspasienter.All(i => i.Status == IndekspasientStatus.Slettet).Should().BeTrue();
        }

        private SimulaKontaktrapport LagSimulaKontaktrapport(string pasientTlf, params string[] kontaktTlfer)
        {
            return new SimulaKontaktrapport
            {
                Telefonnummer = pasientTlf,
                Ferdig = true,
                SistAktivTidspunkt = DateTime.Now.AddMinutes(20),
                Kontakter = kontaktTlfer.Select(tlf => new SimulaKontakt
                {
                    Telefonnummer = tlf,
                    Verifiseringskode = "test123",
                    Versjonsinfo = new SimulaKontaktVersjonsinfo
                    {
                        Pipeline = "test",
                        Enhet = new List<string> { "Motorola DynaTAC" }
                    },
                    Oppsummering = new SimulaKontaktOppsummering
                    {
                        AllKontakt = new SimulaKontaktOppsummering.All
                        {
                            AntallKontakter = 2,
                            AntallDagerMedKontakt = 1,
                            Risikokategori = "high",
                            SoylePlotBase64Png = "dGVzdA==",
                            Interessepunkter = new Dictionary<string, double>()
                        },
                        GpsKontakt = new SimulaKontaktOppsummering.GpsInfo
                        {
                            AkkumulertVarighet = 3600,
                            AkkumulertRisikoscore = 213,
                            AntallDagerMedKontakt = 1,
                            HistogramBase64Png = "dGVzdA=="
                        },
                        BluetoothKontakt = new SimulaKontaktOppsummering.BluetoothInfo
                        {
                            AkkumulertVarighet = 123214,
                            AkkumulertRisikoscore = 4322,
                            RelativtNarVarighet = 12.23,
                            VeldigNarVarighet = 432.23,
                            NarVarighet = 123.21,
                            AntallDagerMedKontakt = 1
                        }
                    },
                    Detaljer = new[]
                        {
                            new SimulaKontaktdetaljer
                            {
                                Dato = DateTime.Today.AddDays(-1),
                                AllKontakt = new SimulaKontaktdetaljer.All
                                {
                                    OppsummertPlotHtml = "<div>Here be plot</div>",
                                    Interessepunkter = new Dictionary<string, double>()
                                },
                                GpsKontakt = new SimulaKontaktdetaljer.PerType
                                {
                                    AkkumulertVarighet = 1235,
                                    AkkumulertRisiko = 3242,
                                    Medianavstand = 12
                                },
                                BluetoothKontakt = new SimulaKontaktdetaljer.BluetoothInfo
                                {
                                    AkkumulertVarighet = 1235,
                                    AkkumulertRisiko = 3242,
                                    Medianavstand = 12,
                                    RelativtNarVarighet = 12.23,
                                    VeldigNarVarighet = 432.23,
                                    NarVarighet = 123.21
                                }
                            }
                        }
                }).ToList()
            };
        }
    }
}