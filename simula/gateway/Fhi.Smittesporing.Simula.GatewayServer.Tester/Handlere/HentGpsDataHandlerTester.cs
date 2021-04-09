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
    public class HentGpsDataHandlerTester
    {
        [Fact]
        public async Task Handle_GittHenvendelse_SenderAlleParameterVidereTilKlient()
        {
            var gpsFra = DateTime.Now.AddDays(-3);
            var gpsTil = DateTime.Now.AddDays(-3).AddMinutes(5);

            var simulaKlientMock = new Mock<ISimulaEksternApiKlient>();
            simulaKlientMock
                .Setup(x => x.HentGpsData(It.IsAny<SimulaGpsDataEgressRequest>()))
                .ReturnsAsync((SimulaGpsDataEgressRequest r) => new SimulaEventListResponseBuilder<SimulaGpsDataEgressEvent>(
                        r, (i, t) => new SimulaGpsDataEgressEventBuilder().Build())
                    .WithEventsCount(1)
                    .Build());

            var target = new HentGpsDataHandler(simulaKlientMock.Object);

            await target.Handle(new HentGpsDataCommand
            {
                Henvendelse = new SimulaGpsData.HentCommand
                {
                    FraTidspunkt = gpsFra,
                    TilTidspunkt = gpsTil,
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

            simulaKlientMock.Verify(x => x.HentGpsData(It.Is<SimulaGpsDataEgressRequest>(r => 
                r.TimeFrom == gpsFra &&
                r.TimeTo == gpsTil &&
                r.LegalMeans == "Testing" &&
                r.PersonId == "24120112345" &&
                r.PersonName == "Mr U. Tester" &&
                r.PersonOrganization == "Innsynsorg ASA" &&
                r.PhoneNumber == "+4798765432" &&
                r.PageNumber == 1 && // Simula bruker 1 index paging
                r.PerPage == 10)));
        }

        [Fact]
        public async Task Handle_GittGpsDataTilbake_ReturnererResultatMedAlleData()
        {
            var gpsFra = DateTime.Now.AddDays(-3);
            var gpsTil = DateTime.Now.AddDays(-3).AddMinutes(5);

            var simulaKlientMock = new Mock<ISimulaEksternApiKlient>();
            simulaKlientMock
                .Setup(x => x.HentGpsData(It.IsAny<SimulaGpsDataEgressRequest>()))
                .ReturnsAsync((SimulaGpsDataEgressRequest r) => new SimulaEventListResponseBuilder<SimulaGpsDataEgressEvent>(
                        r, (i, t) => new SimulaGpsDataEgressEvent
                        {
                            TimeFrom = gpsFra,
                            TimeTo = gpsTil,
                            Accuracy = 12,
                            Altitude = 2469,
                            AltitudeAccuracy = 15,
                            Latitude = 60.391262,
                            Longitude = 5.322054,
                            Speed = 42
                        })
                    .WithEventsCount(1)
                    .Build());

            var target = new HentGpsDataHandler(simulaKlientMock.Object);

            var result = await target.Handle(new HentGpsDataCommand
            {
                Henvendelse = new SimulaGpsData.HentCommand
                {
                    TilTidspunkt = DateTime.Now.AddDays(-1),
                    FraTidspunkt = DateTime.Now,
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

            var gpsEvent = result.Resultater.First();
            gpsEvent.FraTidspunkt.Should().Be(gpsFra);
            gpsEvent.TilTidspunkt.Should().Be(gpsTil);
            gpsEvent.Lengdegrad.Should().Be(5.322054);
            gpsEvent.Breddegrad.Should().Be(60.391262);
            gpsEvent.Noyaktighet.Should().Be(12);
            gpsEvent.Hastighet.Should().Be(42);
            gpsEvent.Hoyde.Should().Be(2469);
            gpsEvent.HoydeNoyaktighet.Should().Be(15);
        }
    }
}