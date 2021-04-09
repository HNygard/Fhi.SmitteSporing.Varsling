using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Datalag;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Preg;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.Indekspasienter
{
    public class ForsokOpprettTester : DomeneTestBase
    {
        [Fact]
        [Trait("Indekspasient", "Opprett")]
        public async Task Handle_GittGyldigPasient_OppretterPasient()
        {
            //arrange
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<FunksjonsbrytereKonfig>, FunksjonsbrytereKonfig>(x => x.Value)
                .Returns(new FunksjonsbrytereKonfig());

            automocker.Use<ITelefonNormalFacade>(new TelefonNormalFacade());

            SetupGyldigPerson(automocker);

            SetupGyldigKontakt(automocker);

            automocker.Setup<ICryptoManagerFacade, string>(x => x.KrypterUtenBrukerinnsyn(It.IsAny<string>()))
                .Returns((string value) => $"<kryptert:{value}>");

            var target = automocker.CreateInstance<ForsokOpprett.Handler>();

            var opprettetTidspunkt = DateTime.Now;
            var provedato = DateTime.Now.AddDays(-1);
            var request = new ForsokOpprett.Command
            {
                Fodselsnummer = "23022312345",
                Opprettettidspunkt = opprettetTidspunkt,
                Provedato = provedato,
                Bostedkommunenummer = "7357",
                Bostedkommune = "Testun"
            };

            //act
            var opprettet = await target.Handle(request, CancellationToken);

            //assert
            opprettet.Should().BeTrue();
            automocker.Verify<IIndekspasientRepository>(x => x.Opprett(It.Is<Indekspasient>(i => 
                i.Fodselsnummer == "<kryptert:23022312345>" &&
                i.Status == IndekspasientStatus.Registrert &&
                i.Kommune.KommuneNr == "7357" &&
                i.Kommune.Navn == "Testun" &&
                i.Opprettettidspunkt == opprettetTidspunkt &&
                i.Provedato == provedato)));
        }

        [Fact]
        [Trait("Indekspasient", "Opprett")]
        public async Task Handle_GittGyldigPasient_OppretterPasientMedNormalisertTlf()
        {
            //arrange
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<FunksjonsbrytereKonfig>, FunksjonsbrytereKonfig>(x => x.Value)
                .Returns(new FunksjonsbrytereKonfig());

            automocker.Use<ITelefonNormalFacade>(new TelefonNormalFacade());

            SetupGyldigPerson(automocker);

            SetupGyldigKontakt(automocker, "98765432");

            automocker.Setup<ICryptoManagerFacade, string>(x => x.KrypterUtenBrukerinnsyn(It.IsAny<string>()))
                .Returns((string value) => $"<kryptert:{value}>");

            var target = automocker.CreateInstance<ForsokOpprett.Handler>();

            var opprettetTidspunkt = DateTime.Now;
            var provedato = DateTime.Now.AddDays(-1);
            var request = new ForsokOpprett.Command
            {
                Fodselsnummer = "23022312345",
                Opprettettidspunkt = opprettetTidspunkt,
                Provedato = provedato,
                Bostedkommunenummer = "7357",
                Bostedkommune = "Testun"
            };

            //act
            var opprettet = await target.Handle(request, CancellationToken);

            //assert
            opprettet.Should().BeTrue();
            automocker.Verify<IIndekspasientRepository>(x => x.Opprett(It.Is<Indekspasient>(i =>
                i.Telefon.Telefonnummer == "<kryptert:+4798765432>")));
        }

        [Fact]
        [Trait("Indekspasient", "Opprett")]
        public async Task Handle_GittTomtFnr_OppretterIkkePasient()
        {
            //arrange
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<FunksjonsbrytereKonfig>, FunksjonsbrytereKonfig>(x => x.Value)
                .Returns(new FunksjonsbrytereKonfig());

            automocker.Use<ITelefonNormalFacade>(new TelefonNormalFacade());

            var target = automocker.CreateInstance<ForsokOpprett.Handler>();

            var request = new ForsokOpprett.Command
            {
                Fodselsnummer = string.Empty,
                Opprettettidspunkt = DateTime.Now,
                Provedato = DateTime.Now
            };

            //act
            var opprettet = await target.Handle(request, CancellationToken);

            //assert
            opprettet.Should().BeFalse();
            automocker.Verify<IIndekspasientRepository>(x => x.Opprett(It.IsAny<Indekspasient>()), Times.Never);
        }

        [Theory]
        [Trait("Indekspasient", "Opprett")]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task Handle_UgyldigPersonGyldigKontakt_OppretterIkkePasient(bool hemmeligAdresse, bool under16)
        {
            //arrange
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<FunksjonsbrytereKonfig>, FunksjonsbrytereKonfig>(x => x.Value)
                .Returns(new FunksjonsbrytereKonfig());

            automocker.Use<ITelefonNormalFacade>(new TelefonNormalFacade());

            automocker
                .Setup<IPregFacade, Task<Option<PregPerson>>>(x => x.FinnPerson(It.IsAny<string>()))
                .ReturnsAsync((string id) => new PregPerson
                {
                    Fodselsdato = DateTime.Now.AddYears(-16).AddDays(under16 ? 1 : -1).Some(),
                    HarHemmeligAdresse = hemmeligAdresse,
                    Identifikator = id
                }.Some());

            SetupGyldigKontakt(automocker);

            var target = automocker.CreateInstance<ForsokOpprett.Handler>();

            var request = new ForsokOpprett.Command
            {
                Fodselsnummer = "23022312345",
                Opprettettidspunkt = DateTime.Now,
                Provedato = DateTime.Now
            };

            //act
            var opprettet = await target.Handle(request, CancellationToken);

            //assert
            opprettet.Should().BeFalse();
            automocker.Verify<IIndekspasientRepository>(
                x => x.Opprett(It.IsAny<Indekspasient>()),
                Times.Never);
        }

        [Fact]
        [Trait("Indekspasient", "Opprett")]
        public async Task Handle_GyldigPersonUgyldigKontaktTillatt_OppretterPasientMedManglerKontaktinfo()
        {
            //arrange
            var automocker = new AutoMocker();

            automocker.Use<ITelefonNormalFacade>(new TelefonNormalFacade());

            automocker.Setup<IOptions<FunksjonsbrytereKonfig>, FunksjonsbrytereKonfig>(x => x.Value)
                .Returns(new FunksjonsbrytereKonfig
                {
                    TillatAngiKontaktinfoManuelt = true
                });

            SetupGyldigPerson(automocker);

            automocker
                .Setup<IKontaktInfoFacade, Task<KontaktinformasjonReponse>>(x => x.HentPersoner(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((IEnumerable<string> ider) => new KontaktinformasjonReponse
                {
                    Status = StatusKoder.Ok,
                    Kontaktinformasjon = ider.Select(id => new Kontaktinformasjon
                    {
                        Fnr = id,
                        Mobil = null
                    }).ToList(),
                    FeilInfo = null
                });

            var target = automocker.CreateInstance<ForsokOpprett.Handler>();

            var request = new ForsokOpprett.Command
            {
                Fodselsnummer = "23022312345",
                Opprettettidspunkt = DateTime.Now,
                Provedato = DateTime.Now
            };

            //act
            var opprettet = await target.Handle(request, CancellationToken);

            //assert
            opprettet.Should().BeTrue();
            automocker.Verify<IIndekspasientRepository>(x => x.Opprett(It.Is<Indekspasient>(i =>
                i.Status == IndekspasientStatus.KontaktInfoMangler)));
        }

        [Theory]
        [Trait("Indekspasient", "Opprett")]
        [InlineData(true, PersonStatus.Aktiv)]
        [InlineData(true, PersonStatus.IkkeRegistrert)]
        [InlineData(true, PersonStatus.Slettet)]
        [InlineData(false, PersonStatus.IkkeRegistrert)]
        [InlineData(false, PersonStatus.Slettet)]
        public async Task Handle_GyldigPersonUgyldigKontakt_OppretterIkkePasient(bool manglerMobil, PersonStatus status)
        {
            //arrange
            var automocker = new AutoMocker();

            automocker.Use<ITelefonNormalFacade>(new TelefonNormalFacade());

            automocker.Setup<IOptions<FunksjonsbrytereKonfig>, FunksjonsbrytereKonfig>(x => x.Value)
                .Returns(new FunksjonsbrytereKonfig());

            SetupGyldigPerson(automocker);

            automocker
                .Setup<IKontaktInfoFacade, Task<KontaktinformasjonReponse>>(x => x.HentPersoner(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((IEnumerable<string> ider) => new KontaktinformasjonReponse
                {
                    Status = StatusKoder.Ok,
                    Kontaktinformasjon = ider.Select(id => new Kontaktinformasjon
                    {
                        Fnr = id,
                        Mobil = manglerMobil ? null : "98765432",
                        Status = status
                    }).ToList(),
                    FeilInfo = null
                });

            var target = automocker.CreateInstance<ForsokOpprett.Handler>();

            var request = new ForsokOpprett.Command
            {
                Fodselsnummer = "23022312345",
                Opprettettidspunkt = DateTime.Now,
                Provedato = DateTime.Now
            };

            //act
            var opprettet = await target.Handle(request, CancellationToken);

            //assert
            opprettet.Should().BeFalse();
            automocker.Verify<IIndekspasientRepository>(
                x => x.Opprett(It.IsAny<Indekspasient>()),
                Times.Never);
        }


        [Fact]
        [Trait("Indekspasient", "Opprett")]
        public async Task Handle_GittGyldigPasientUnntakFraKontaktinfo_KasterUnntakVidere()
        {
            //arrange
            var automocker = new AutoMocker();

            automocker.Use<ITelefonNormalFacade>(new TelefonNormalFacade());

            automocker.Setup<IOptions<FunksjonsbrytereKonfig>, FunksjonsbrytereKonfig>(x => x.Value)
                .Returns(new FunksjonsbrytereKonfig());

            SetupGyldigPerson(automocker);

            automocker
                .Setup<IKontaktInfoFacade, Task<KontaktinformasjonReponse>>(x => x.HentPersoner(It.IsAny<IEnumerable<string>>()))
                .ThrowsAsync(new Exception("test"));

            var target = automocker.CreateInstance<ForsokOpprett.Handler>();

            var request = new ForsokOpprett.Command
            {
                Fodselsnummer = "23022312345",
                Opprettettidspunkt = DateTime.Now,
                Provedato = DateTime.Now
            };

            //act
            await Assert.ThrowsAsync<Exception>(() => target.Handle(request, CancellationToken));
        }

        [Fact]
        [Trait("Indekspasient", "Opprett")]
        public async Task Handle_GittGyldigKontaktinfoUnntakFraPreg_KasterUnntakVidere()
        {
            //arrange
            var automocker = new AutoMocker();

            automocker.Use<ITelefonNormalFacade>(new TelefonNormalFacade());

            automocker.Setup<IOptions<FunksjonsbrytereKonfig>, FunksjonsbrytereKonfig>(x => x.Value)
                .Returns(new FunksjonsbrytereKonfig());

            automocker
                .Setup<IPregFacade, Task<Option<PregPerson>>>(x => x.FinnPerson(It.IsAny<string>()))
                .ThrowsAsync(new Exception("test"));

            SetupGyldigKontakt(automocker);

            var target = automocker.CreateInstance<ForsokOpprett.Handler>();

            var request = new ForsokOpprett.Command
            {
                Fodselsnummer = "23022312345",
                Opprettettidspunkt = DateTime.Now,
                Provedato = DateTime.Now
            };

            //act
            await Assert.ThrowsAsync<Exception>(() => target.Handle(request, CancellationToken));
        }

        private void SetupGyldigPerson(AutoMocker automocker)
        {
            automocker
                .Setup<IPregFacade, Task<Option<PregPerson>>>(x => x.FinnPerson(It.IsAny<string>()))
                .ReturnsAsync((string id) => new PregPerson
                {
                    Fodselsdato = DateTime.Now.AddYears(-17).Some(),
                    HarHemmeligAdresse = false,
                    Identifikator = id
                }.Some());
        }

        private void SetupGyldigKontakt(AutoMocker automocker, string tlf = "+4798765432")
        {
            automocker
                .Setup<IKontaktInfoFacade, Task<KontaktinformasjonReponse>>(x => x.HentPersoner(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((IEnumerable<string> ider) => new KontaktinformasjonReponse
                {
                    Status = StatusKoder.Ok,
                    Kontaktinformasjon = ider.Select(id => new Kontaktinformasjon
                    {
                        Fnr = id,
                        Mobil = tlf,
                        Status = PersonStatus.Aktiv
                    }).ToList(),
                    FeilInfo = null
                });
        }
    }
}