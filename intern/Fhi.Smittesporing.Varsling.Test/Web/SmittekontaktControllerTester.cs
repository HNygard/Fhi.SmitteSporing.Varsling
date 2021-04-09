using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Smittekontakter;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
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
    public class SmittekontaktControllerTester : ControllerTestBase
    {
        [Fact]
        public async Task HentPersonopplysninger_GittPasientFinnes_BrukerRiktigQueryMedBrukernavnOgGirGyldigReturType()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<Option<SmittekontaktPersonopplysningerAm>>>(x => x.Send(It.IsAny<HentPersonopplysninger.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SmittekontaktPersonopplysningerAm
                {
                    Telefonnummer = "+4798765432"
                }.Some());

            var target = automocker.CreateInstance<SmittekontaktController>();
            target.ControllerContext = LagControllerContextMedBruker("test/Ola");

            var actionResult = await target.HentPersonopplysninger(42);

            actionResult.Should().NotBeNull();
            actionResult.Value.Telefonnummer.Should().Be("+4798765432");

            automocker.Verify<IMediator>(x => x.Send(It.Is<HentPersonopplysninger.Query>(q =>
                q.Brukernavn == "test/Ola" &&
                q.SmittekontaktId == 42), It.IsAny<CancellationToken>()));
        }



        [Fact]
        public async Task HentPersonopplysninger_GittSmittekontaktFinnesIkke_GirNotFound()
        {
            var automocker = new AutoMocker();

            automocker
                .Setup<IMediator, Task<Option<SmittekontaktPersonopplysningerAm>>>(x => x.Send(It.IsAny<HentPersonopplysninger.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Option.None<SmittekontaktPersonopplysningerAm>());

            var target = automocker.CreateInstance<SmittekontaktController>();
            target.ControllerContext = LagControllerContextMedBruker();

            var actionResult = await target.HentPersonopplysninger(123);

            actionResult.Should().NotBeNull();
            actionResult.Value.Should().BeNull();
            ((StatusCodeResult)actionResult.Result).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}