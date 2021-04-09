using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Datalag;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.Indekspasienter
{
    public class RegistrerTelefonnummerTester
    {
        [Fact]
        public async Task Handle_GittKontaktInfoIkkeFunnet_SletterIndekspasient()
        {
            var indekspasient =
                new Indekspasient
                {
                    IndekspasientId = 12,
                    TelefonId = null,
                    Telefon = null,
                    Fodselsnummer = "krypt123asd123",
                    Status = IndekspasientStatus.KontaktInfoMangler,
                    Kommune = new Kommune
                    {
                        KommuneId = 12,
                        Navn = "Asd"
                    },
                    KommuneId = 12,
                    Varslingsstatus = Varslingsstatus.TilGodkjenning,
                    Opprettettidspunkt = DateTime.Now.AddDays(-1),
                    Provedato = DateTime.Now.AddDays(-2),
                    Created = DateTime.Now,
                    Smittekontakter = new List<Smittekontakt>()
                };

            var automocker = new AutoMocker();

            automocker
                .Setup<IIndekspasientRepository, Task<Option<Indekspasient>>>(x => x.HentForIdInkluderTelefon(42))
                .ReturnsAsync(indekspasient.Some());

            var target = automocker.CreateInstance<RegistrerTelefonnummer.Handler>();

            await target.Handle(new RegistrerTelefonnummer.Command
            {
                Telefonnummer = null,
                IndekspasientId = 42,
                IkkeManueltFunnetKontaktInfo = true
            }, new CancellationToken());

            indekspasient.Status.Should().Be(IndekspasientStatus.Slettet);
            indekspasient.Fodselsnummer.Should().BeNull();
            indekspasient.Provedato.Should().BeNull();
            indekspasient.KommuneId.Should().BeNull();
            indekspasient.Kommune.Should().BeNull();
            indekspasient.TelefonId.Should().BeNull();
            indekspasient.Telefon.Should().BeNull();
            indekspasient.Fodselsnummer.Should().BeNull();
            automocker.Verify<IIndekspasientRepository>(x => x.Lagre());
        }

        [Fact]
        public async Task Handle_GittKontakt_OppdaterePasient()
        {
            var indekspasient =
                new Indekspasient
                {
                    IndekspasientId = 12,
                    TelefonId = null,
                    Telefon = null,
                    Fodselsnummer = "krypt123asd123",
                    Status = IndekspasientStatus.KontaktInfoMangler
                };

            var automocker = new AutoMocker();

            automocker
                .Setup<IIndekspasientRepository, Task<Option<Indekspasient>>>(x => x.HentForIdInkluderTelefon(42))
                .ReturnsAsync(indekspasient.Some());
            automocker.Use<ITelefonNormalFacade>(new TelefonNormalFacade());
            automocker
                .Setup<ICryptoManagerFacade, string>(x => x.KrypterUtenBrukerinnsyn(It.IsAny<string>()))
                .Returns((string v) => "kryptert:" + v);

            var target = automocker.CreateInstance<RegistrerTelefonnummer.Handler>();

            await target.Handle(new RegistrerTelefonnummer.Command
            {
                Telefonnummer = "98765432",
                IndekspasientId = 42,
                IkkeManueltFunnetKontaktInfo = false
            }, new CancellationToken());

            indekspasient.Status.Should().Be(IndekspasientStatus.Registrert);
            indekspasient.Telefon.Telefonnummer.Should().Be("kryptert:+4798765432");
            automocker.Verify<IIndekspasientRepository>(x => x.Lagre());
        }
    }
}