using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Rapport;
using Fhi.Smittesporing.Varsling.Intern.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Optional;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Web
{
    public class IndekspasienterControllerTester : ControllerTestBase
    {
        [Fact]
        public async Task HentListe_BrukerRiktigQueryOgGirGyldigReturType()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<PagedListAm<IndekspasientMedAntallAm>>>(x => x.Send(It.IsAny<HentListe.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedListAm<IndekspasientMedAntallAm>());

            var target = automocker.CreateInstance<IndekspasientController>();
            target.ControllerContext = LagControllerContextMedBruker("test/Ola");

            var actionResult = await target.HentListe(new IndekspasientAm.Filter
            {
                MedSmittekontakt = true
            });

            Assert.NotNull(actionResult);
            Assert.NotNull(actionResult.Value);

            automocker.Verify<IMediator>(x => x.Send(It.Is<HentListe.Query>(q =>
                q.Brukernavn == "test/Ola" &&
                q.Filter.MedSmittekontakt == true), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HentRapport_BrukerRiktigQueryOgGirGyldigReturType()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<IndekspasientRapportAm>>(x => x.Send(It.IsAny<HentRapport.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IndekspasientRapportAm());

            var target = automocker.CreateInstance<IndekspasientController>();
            target.ControllerContext = LagControllerContextMedBruker();

            var actionResult = await target.HentRapport(new IndekspasientRapportAm.Filter
            {
                KommuneNr = "123"
            });

            Assert.NotNull(actionResult);
            Assert.NotNull(actionResult.Value);

            automocker.Verify<IMediator>(x => x.Send(It.Is<HentRapport.Query>(q =>
                q.Filter.KommuneNr == "123"), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HentForId_GittPasientFinnes_BrukerRiktigQueryOgGirGyldigReturType()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<Option<IndekspasientMedAntallAm>>>(x => x.Send(It.IsAny<HentForId.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IndekspasientMedAntallAm
                {
                    IndekspasientId = 42
                }.Some());

            var target = automocker.CreateInstance<IndekspasientController>();
            target.ControllerContext = LagControllerContextMedBruker();

            var actionResult = await target.HentForId(123);

            actionResult.Should().NotBeNull();
            actionResult.Value.IndekspasientId.Should().Be(42);
        }

        [Fact]
        public async Task HentForId_GittPasientFinnesIkke_GirNotFound()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<Option<IndekspasientMedAntallAm>>>(x => x.Send(It.IsAny<HentForId.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Option.None<IndekspasientMedAntallAm>());

            var target = automocker.CreateInstance<IndekspasientController>();
            target.ControllerContext = LagControllerContextMedBruker();

            var actionResult = await target.HentForId(123);

            actionResult.Should().NotBeNull();
            actionResult.Value.Should().BeNull();
            ((StatusCodeResult)actionResult.Result).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task HentPersonopplysninger_GittPasientFinnes_BrukerRiktigQueryMedBrukernavnOgGirGyldigReturType()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<Option<IndekspasientPersonopplysningerAm>>>(x => x.Send(It.IsAny<HentPersonopplysninger.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IndekspasientPersonopplysningerAm
                {
                    Telefonnummer = "+4798765432"
                }.Some());

            var target = automocker.CreateInstance<IndekspasientController>();
            target.ControllerContext = LagControllerContextMedBruker("test/Ola");

            var actionResult = await target.HentPersonopplysningerForId(42);

            actionResult.Should().NotBeNull();
            actionResult.Value.Telefonnummer.Should().Be("+4798765432");


            automocker.Verify<IMediator>(x => x.Send(It.Is<HentPersonopplysninger.Query>(q =>
                q.Brukernavn == "test/Ola" &&
                q.IndekspasientId == 42), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task HentPersonopplysninger_GittPasientFinnesIkke_GirNotFound()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<Option<IndekspasientPersonopplysningerAm>>>(x => x.Send(It.IsAny<HentPersonopplysninger.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Option.None<IndekspasientPersonopplysningerAm>());

            var target = automocker.CreateInstance<IndekspasientController>();
            target.ControllerContext = LagControllerContextMedBruker();

            var actionResult = await target.HentPersonopplysningerForId(123);

            actionResult.Should().NotBeNull();
            actionResult.Value.Should().BeNull();
            ((StatusCodeResult)actionResult.Result).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Theory]
        [Trait("ManglerKontaktinfo", "RegistrerTelefon")]
        [InlineData("12345678901", false)]
        [InlineData(null, true)]
        public async Task RegistrerTelefon(string tlf, bool ikkeFunnet)
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<Unit>>(x => x.Send(It.IsAny<RegistrerTelefonnummer.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var target = automocker.CreateInstance<IndekspasientController>();
            target.ControllerContext = LagControllerContextMedBruker();

            var actionResult = await target.RegistrerTelefon(42, tlf, ikkeFunnet);

            Assert.NotNull(actionResult);

            automocker.Verify<IMediator>(x => x.Send(It.Is<RegistrerTelefonnummer.Command>(c =>
                c.IndekspasientId == 42 &&
                c.Telefonnummer == tlf &&
                c.IkkeManueltFunnetKontaktInfo == ikkeFunnet
            ), It.IsAny<CancellationToken>()));
        }


        [Fact]
        public async Task Varslingssimulering_BrukerRiktigQueryOgReturtype()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<VarslingssimuleringAm>>(x => x.Send(It.IsAny<HentVarslingssimulering.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VarslingssimuleringAm
                {
                    IndekspasientId = 42
                });

            var target = automocker.CreateInstance<IndekspasientController>();
            target.ControllerContext = LagControllerContextMedBruker("test/Ola");

            var actionResult = await target.HentVarslingssimulering(42);

            actionResult.Should().NotBeNull();
            actionResult.Value.IndekspasientId.Should().Be(42);
        }
    }

}
