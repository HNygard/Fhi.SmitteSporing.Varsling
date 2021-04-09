using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Smittekontakter;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Optional;
using Optional.Unsafe;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.Smittekontakter
{
    public class HentDagDetaljerKartSomHtmlTester
    {
        [Fact]
        public async Task Handle_GittDagDetaljerFinnesIkke_ReturnererNone()
        {
            var automocker = new AutoMocker();

            automocker.Setup<ISmittekontaktRespository, Task<Option<SmittekontaktDetaljer>>>(x =>
                    x.HentDetaljerForDagMedHtmlKartOgTelefon(12, 42))
                .ReturnsAsync(() => Option.None<SmittekontaktDetaljer>());

            var target = automocker.CreateInstance<HentDagDetaljerKartSomHtml.Handler>();

            var result = await target.Handle(new HentDagDetaljerKartSomHtml.Query
            {
                SmittekontaktId = 12,
                SmittekontaktDagDetaljerId = 42,
                Brukernavn = "ola",
            }, new CancellationToken());

            result.Should().Be(Option.None<string>());
        }

        [Fact]
        public async Task Handle_GittDagDetaljerHarIkkeKart_ReturnererHtmlMedInfoOmIngenKart()
        {
            var automocker = new AutoMocker();

            automocker.Setup<ISmittekontaktRespository, Task<Option<SmittekontaktDetaljer>>>(x =>
                    x.HentDetaljerForDagMedHtmlKartOgTelefon(12, 42))
                .ReturnsAsync(() => new SmittekontaktDetaljer
                {
                    OppsummertPlotDetaljerHtml = null
                }.Some());

            var target = automocker.CreateInstance<HentDagDetaljerKartSomHtml.Handler>();

            var result = await target.Handle(new HentDagDetaljerKartSomHtml.Query
            {
                SmittekontaktId = 12,
                SmittekontaktDagDetaljerId = 42,
                Brukernavn = "ola",
            }, new CancellationToken());

            result.HasValue.Should().BeTrue();

            result.ValueOrDefault().Should().Contain("Kontakthendelse har ikke tilgjengelig kart.");
        }



        [Fact]
        public async Task Handle_GittDagDetaljerHarNoGpsMeldingForKart_ReturnererHtmlMedNoGpsInfo()
        {
            var automocker = new AutoMocker();

            automocker.Setup<ISmittekontaktRespository, Task<Option<SmittekontaktDetaljer>>>(x =>
                    x.HentDetaljerForDagMedHtmlKartOgTelefon(12, 42))
                .ReturnsAsync(() => new SmittekontaktDetaljer
                {
                    OppsummertPlotDetaljerHtml = new SmittekontaktDetaljerHtmlKart
                    {
                        Innhold = "<kryptert-kart>"
                    },
                    Smittekontakt = new Smittekontakt
                    {
                        Telefon = new Telefon
                        {
                            Telefonnummer = "<kryptert-tlf>"
                        }
                    }
                }.Some());

            automocker
                .Setup<ICryptoManagerFacade, string>(x => x.DekrypterDataTilknyttet("<kryptert-kart>", "<kryptert-tlf>", "Telefonnummer", "Kvalitetssikring av kontaktrapport", "ola"))
                .Returns("No GPS information");

            var target = automocker.CreateInstance<HentDagDetaljerKartSomHtml.Handler>();

            var result = await target.Handle(new HentDagDetaljerKartSomHtml.Query
            {
                SmittekontaktId = 12,
                SmittekontaktDagDetaljerId = 42,
                Brukernavn = "ola",
            }, new CancellationToken());

            result.HasValue.Should().BeTrue();

            result.ValueOrDefault().Should().Contain("No GPS information");
        }



        [Fact]
        public async Task Handle_GittDagDetaljerGammelVersjon_ReturnererHtmlMedNoGpsInfo()
        {
            var automocker = new AutoMocker();

            automocker.Setup<ISmittekontaktRespository, Task<Option<SmittekontaktDetaljer>>>(x =>
                    x.HentDetaljerForDagMedHtmlKartOgTelefon(12, 42))
                .ReturnsAsync(() => new SmittekontaktDetaljer
                {
                    OppsummertPlotDetaljerHtml = new SmittekontaktDetaljerHtmlKart
                    {
                        Innhold = "<kryptert-html-kart>"
                    },
                    Smittekontakt = new Smittekontakt
                    {
                        Telefon = new Telefon
                        {
                            Telefonnummer = "<kryptert-tlf>"
                        }
                    }
                }.Some());

            automocker
                .Setup<ICryptoManagerFacade, string>(x => x.DekrypterDataTilknyttet("<kryptert-html-kart>", "<kryptert-tlf>", "Telefonnummer", "Kvalitetssikring av kontaktrapport", "ola"))
                .Returns("<div>Interaktivt html kart</div>");

            var target = automocker.CreateInstance<HentDagDetaljerKartSomHtml.Handler>();

            var result = await target.Handle(new HentDagDetaljerKartSomHtml.Query
            {
                SmittekontaktId = 12,
                SmittekontaktDagDetaljerId = 42,
                Brukernavn = "ola",
            }, new CancellationToken());

            result.HasValue.Should().BeTrue();

            result.ValueOrDefault().Should().Contain("<div>Interaktivt html kart</div>");
        }


        [Fact]
        public async Task Handle_GittDagDetaljerBas64Bilde_ReturnererHtmlBilde()
        {
            var automocker = new AutoMocker();

            automocker.Setup<ISmittekontaktRespository, Task<Option<SmittekontaktDetaljer>>>(x =>
                    x.HentDetaljerForDagMedHtmlKartOgTelefon(12, 42))
                .ReturnsAsync(() => new SmittekontaktDetaljer
                {
                    OppsummertPlotDetaljerHtml = new SmittekontaktDetaljerHtmlKart
                    {
                        Innhold = "<kryptert-kart-png>"
                    },
                    Smittekontakt = new Smittekontakt
                    {
                        Telefon = new Telefon
                        {
                            Telefonnummer = "<kryptert-tlf>"
                        }
                    }
                }.Some());

            automocker
                .Setup<ICryptoManagerFacade, string>(x => x.DekrypterDataTilknyttet("<kryptert-kart-png>", "<kryptert-tlf>", "Telefonnummer", "Kvalitetssikring av kontaktrapport", "ola"))
                .Returns("iVBORw0==");

            var target = automocker.CreateInstance<HentDagDetaljerKartSomHtml.Handler>();

            var result = await target.Handle(new HentDagDetaljerKartSomHtml.Query
            {
                SmittekontaktId = 12,
                SmittekontaktDagDetaljerId = 42,
                Brukernavn = "ola",
            }, new CancellationToken());

            result.HasValue.Should().BeTrue();

            result.ValueOrDefault().Should().Contain("src=\"data:image/png;base64,iVBORw0==\"");
        }
    }
}