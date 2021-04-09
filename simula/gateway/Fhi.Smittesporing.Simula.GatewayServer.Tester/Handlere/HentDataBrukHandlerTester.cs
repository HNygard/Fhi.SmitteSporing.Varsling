using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.EksternKlient;
using Fhi.Smittesporing.Simula.EksternKlient.Mock;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Fhi.Smittesporing.Simula.GatewayServer.Handlers;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using FluentAssertions;
using Moq;
using Xunit;

namespace Fhi.Smittesporing.Simula.GatewayServer.Tester.Handlere
{
    public class HentDataBrukHandlerTester
    {
        [Fact]
        public async Task Handle_GittHenvendelse_SenderAlleParameterVidereTilKlient()
        {
            var simulaKlientMock = new Mock<ISimulaEksternApiKlient>();
            simulaKlientMock
                .Setup(x => x.HentTilgangslogg(It.IsAny<SimulaTransparencyRequest>()))
                .ReturnsAsync((SimulaTransparencyRequest r) => new SimulaEventListResponseBuilder<SimulaAccessLogEvent>(
                        r, (i, t) => new SimulaAccessLogEventBuilder().Build())
                    .WithEventsCount(1)
                    .Build());

            var target = new HentDataBrukHandler(simulaKlientMock.Object);

            await target.Handle(new HentDataBrukCommand
            {
                Henvendelse = new SimulaDataBruk.HentCommand
                {
                    Sideantall = 10,
                    Sideindeks = 0,
                    PersonOrganisasjon = "Innsynsorg ASA",
                    PersonIdentifikator = "24120112345",
                    TilknyttetTelefonnummer = "+4798765432",
                    RettsligFormal = "Testing",
                    PersonNavn = "Mr U. Tester",
                    TekniskOrganisasjon = "XUnit INC"
                }
            }, new CancellationToken());

            simulaKlientMock.Verify(x => x.HentTilgangslogg(It.Is<SimulaTransparencyRequest>(r =>
                r.LegalMeans == "Testing" &&
                r.PersonId == "24120112345" &&
                r.PersonName == "Mr U. Tester" &&
                r.PersonOrganization == "Innsynsorg ASA" &&
                r.PhoneNumber == "+4798765432" &&
                r.PageNumber == 1 && // Simula bruker 1 index paging
                r.PerPage == 10)));
        }

        [Fact]
        public async Task Handle_GittDatabrukTilbake_ReturnererResultatMedAlleData()
        {
            var timestamp = DateTime.Now.AddDays(-3);

            var simulaKlientMock = new Mock<ISimulaEksternApiKlient>();
            simulaKlientMock
                .Setup(x => x.HentTilgangslogg(It.IsAny<SimulaTransparencyRequest>()))
                .ReturnsAsync((SimulaTransparencyRequest r) => new SimulaEventListResponseBuilder<SimulaAccessLogEvent>(
                        r, (i, t) => new SimulaAccessLogEvent
                        {
                            Timestamp = timestamp,
                            PersonOrganization = "Innsynsorg ASA",
                            PersonId = "24120112345",
                            PhoneNumber = "+4798765432",
                            PersonName = "Mr U. Tester",
                            LegalMeans = "Testing",
                            TechnicalOrganization = "XUnit INC"
                        })
                    .WithEventsCount(1)
                    .Build());

            var target = new HentDataBrukHandler(simulaKlientMock.Object);

            var result = await target.Handle(new HentDataBrukCommand
            {
                Henvendelse = new SimulaDataBruk.HentCommand
                {
                    Sideantall = 10,
                    Sideindeks = 0,
                    PersonOrganisasjon = "Innsynsorg ASA",
                    PersonIdentifikator = "24120112345",
                    TilknyttetTelefonnummer = "+4798765432",
                    RettsligFormal = "Testing",
                    PersonNavn = "Mr U. Tester",
                    TekniskOrganisasjon = "XUnit INC"
                }
            }, new CancellationToken());

            result.AntallSider.Should().Be(1);
            result.TotaltAntall.Should().Be(1);
            result.Sideindeks.Should().Be(0);
            result.Sideantall.Should().Be(10);

            var dataBruk = result.Resultater.First();
            dataBruk.Tidspunkt.Should().Be(timestamp);
            dataBruk.PersonIdentifikator.Should().Be("24120112345");
            dataBruk.PersonNavn.Should().Be("Mr U. Tester");
            dataBruk.PersonOrganisasjon.Should().Be("Innsynsorg ASA");
            dataBruk.TekniskOrganisasjon.Should().Be("XUnit INC");
            dataBruk.RettsligFormal.Should().Be("Testing");
            dataBruk.TilknyttetTelefonnummer.Should().Be("+4798765432");
        }
    }
}