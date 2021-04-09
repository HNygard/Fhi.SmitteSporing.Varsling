using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.EksternKlient;
using Fhi.Smittesporing.Simula.EksternKlient.Mock;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Fhi.Smittesporing.Simula.GatewayServer.Handlers;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using FluentAssertions;
using Moq;
using Optional;
using Optional.Unsafe;
using Xunit;

namespace Fhi.Smittesporing.Simula.GatewayServer.Tester.Handlere
{
    public class HentKontaktrapportHandlerTester
    {
        [Fact]
        public async Task Handle_GittResultatIkkeKlart_GirResultatMedStatusIkkeFerdig()
        {
            var simulaKlientMock = new Mock<ISimulaEksternApiKlient>();
            simulaKlientMock
                .Setup(x => x.HentKontaktresultat(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => Option.None<SimulaContactReport, SimulaNotFinishedResult>(new SimulaNotFinishedResult
                {
                    Message = "Soon!"
                }).Some());

            var target = new HentKontaktrapportHandler(simulaKlientMock.Object);

            var maybeResult = await target.Handle(new HentKontaktrapportQuery
            {
                Id = Guid.NewGuid()
            }, new CancellationToken());

            maybeResult.HasValue.Should().BeTrue();

            var result = maybeResult.ValueOrFailure();

            result.Ferdig.Should().BeFalse();
            result.StatusMelding.Should().Be("Soon!");
        }

        [Fact]
        public async Task Handle_GittResultatKlart_GirResultatMedFerdigStatus()
        {
            var simulaKlientMock = new Mock<ISimulaEksternApiKlient>();
            simulaKlientMock
                .Setup(x => x.HentKontaktresultat(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => new SimulaContactReportBuilder()
                    .InSystem()
                    .WithContact(c => c.WithHighRiskScore())
                    .Build().Some<SimulaContactReport, SimulaNotFinishedResult>().Some());

            var target = new HentKontaktrapportHandler(simulaKlientMock.Object);

            var maybeResult = await target.Handle(new HentKontaktrapportQuery
            {
                Id = Guid.NewGuid()
            }, new CancellationToken());

            maybeResult.HasValue.Should().BeTrue();

            var result = maybeResult.ValueOrFailure();

            result.Ferdig.Should().BeTrue();
            result.Telefonnummer.Should().NotBeNullOrEmpty();
            result.Kontakter.Count.Should().Be(1);
            var kontakt = result.Kontakter.First();
            kontakt.Verifiseringskode.Should().NotBeNullOrEmpty();
            kontakt.Versjonsinfo.Should().NotBeNull();
            kontakt.Versjonsinfo.Pipeline.Should().NotBeNull();
            kontakt.Versjonsinfo.Enhet.Should().NotBeNull();
            kontakt.Oppsummering.Should().NotBeNull();
            kontakt.Detaljer.Should().NotBeEmpty();
            var detalj = kontakt.Detaljer.First();
            detalj.AllKontakt.Should().NotBeNull();
            detalj.AllKontakt.Interessepunkter.Should().NotBeNull();
            detalj.BluetoothKontakt.Should().NotBeNull();
            detalj.GpsKontakt.Should().NotBeNull();
        }
    }
}