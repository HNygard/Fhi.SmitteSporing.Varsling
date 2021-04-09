using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.InternKlient;
using Fhi.Smittesporing.Varsling.Eksternetjenester;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Optional;
using Optional.Unsafe;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.EksterneTjenester
{
    public class SimulaFacadeTester
    {
        [Fact]
        public async Task HentKontaktrapport_GittNotFoundVedOpprett_ReturnererNone()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<SimulaFacade.Konfig>, SimulaFacade.Konfig>(x => x.Value)
                .Returns(new SimulaFacade.Konfig());

            automocker.Setup<ISimulaInternKlient, Task<Option<Guid>>>(x =>
                x.OpprettKontaktrapport(It.IsAny<SimulaKontaktrapport.OpprettCommand>()))
                .ReturnsAsync(Option.None<Guid>());

            var target = automocker.CreateInstance<SimulaFacade>();

            var result = await target.GetSmittekontakter("+4798765432", DateTime.Now.AddDays(-1), DateTime.Now);

            result.Should().Be(Option.None<SimulaKontaktrapport>());
        }

        [Fact]
        public async Task HentKontaktrapport_FikkIkkeResultatEtterMaksForsok_GirFeilmelding()
        {
            var automocker = new AutoMocker();

            var rapportGuid = Guid.NewGuid();

            automocker.Setup<IOptions<SimulaFacade.Konfig>, SimulaFacade.Konfig>(x => x.Value)
                .Returns(new SimulaFacade.Konfig
                {
                    MaksAntallForsok = 2,
                    RapportHentingPause = TimeSpan.Zero
                });

            automocker.Setup<ISimulaInternKlient, Task<Option<Guid>>>(x =>
                    x.OpprettKontaktrapport(It.IsAny<SimulaKontaktrapport.OpprettCommand>()))
                .ReturnsAsync(rapportGuid.Some());

            automocker.SetupSequence<ISimulaInternKlient, Task<Option<SimulaKontaktrapport>>>(x =>
                    x.HentKontaktrapport(rapportGuid))
                .ReturnsAsync(() => new SimulaKontaktrapport
                {
                    Ferdig = false
                }.Some())
                .ReturnsAsync(() => new SimulaKontaktrapport
                {
                    Ferdig = true,
                    Telefonnummer = "+4798765432",
                    Kontakter = new List<SimulaKontakt>(),
                    SistAktivTidspunkt = DateTime.Now.AddHours(-1)
                }.Some());

            var target = automocker.CreateInstance<SimulaFacade>();

            var result = await target.GetSmittekontakter("+4798765432", DateTime.Now.AddDays(-1), DateTime.Now);

            result.HasValue.Should().BeTrue();
            result.ValueOrFailure().Telefonnummer.Should().Be("+4798765432");
        }

        [Fact]
        public async Task GetSimulaApiInfo_GittSvarFraBeggeServere_ReturnererVersjonsinfo()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<SimulaFacade.Konfig>, SimulaFacade.Konfig>(x => x.Value)
                .Returns(new SimulaFacade.Konfig());

            automocker.Setup<ISimulaInternKlient, Task<ServerVersjonAm>>(x => x.HentVersjon())
                .ReturnsAsync(new ServerVersjonAm
                {
                    AssemblyVersjon = "1.2.3.4"
                });

            automocker.Setup<ISimulaInternKlient, Task<ServerVersjonAm>>(x => x.HentProxyVersjon())
                .ReturnsAsync(new ServerVersjonAm
                {
                    AssemblyVersjon = "0.1.2.3"
                });

            var target = automocker.CreateInstance<SimulaFacade>();

            var result = await target.GetSimulaApiInfo();

            result.GatewayVersjon.Should().Be("1.2.3.4");
            result.ProxyVersjon.Should().Be("0.1.2.3");
        }

        [Fact]
        public async Task GetSimulaApiInfo_GittGatewayFeiler_ReturnererUtilgjengeligForGateway()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<SimulaFacade.Konfig>, SimulaFacade.Konfig>(x => x.Value)
                .Returns(new SimulaFacade.Konfig());

            automocker.Setup<ISimulaInternKlient, Task<ServerVersjonAm>>(x => x.HentVersjon())
                .ThrowsAsync(new Exception("test"));

            automocker.Setup<ISimulaInternKlient, Task<ServerVersjonAm>>(x => x.HentProxyVersjon())
                .ReturnsAsync(new ServerVersjonAm
                {
                    AssemblyVersjon = "0.1.2.3"
                });

            var target = automocker.CreateInstance<SimulaFacade>();

            var result = await target.GetSimulaApiInfo();

            result.GatewayVersjon.Should().Be("Utilgjengelig");
            result.ProxyVersjon.Should().Be("0.1.2.3");
        }
    }
}