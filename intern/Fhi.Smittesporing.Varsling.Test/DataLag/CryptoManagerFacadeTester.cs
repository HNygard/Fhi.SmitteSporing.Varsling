using Fhi.Smittesporing.Varsling.Datalag;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn.Innsynlogg;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.DataLag
{
    public class CryptoManagerFacadeTester
    {
        [Fact]
        public void DekrypterUtenBrukerinnsyn_GittKryptertTekst_ReturnererDekryptert()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<CryptoManagerFacade.Konfig>, CryptoManagerFacade.Konfig>(x => x.Value)
                .Returns(new CryptoManagerFacade.Konfig
                {
                    UseEmbeddedTestKey = true
                });

            var target = automocker.CreateInstance<CryptoManagerFacade>();

            var result = target.DekrypterUtenBrukerinnsyn("79E2A544964802DE4B2CC345D784A8FB");

            result.Should().Be("test");
        }

        [Fact]
        public void KrypterUtenBrukerinnsyn_GittKlartekst_ReturnererKryptertTekst()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<CryptoManagerFacade.Konfig>, CryptoManagerFacade.Konfig>(x => x.Value)
                .Returns(new CryptoManagerFacade.Konfig
                {
                    UseEmbeddedTestKey = true
                });

            var target = automocker.CreateInstance<CryptoManagerFacade>();

            var result = target.KrypterUtenBrukerinnsyn("test");

            result.Should().Be("79E2A544964802DE4B2CC345D784A8FB");
        }



        [Fact]
        public void KrypterUtenBrukerinnsyn_GittBytes_ReturnererKryptertBytes()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<CryptoManagerFacade.Konfig>, CryptoManagerFacade.Konfig>(x => x.Value)
                .Returns(new CryptoManagerFacade.Konfig
                {
                    UseEmbeddedTestKey = true
                });

            var target = automocker.CreateInstance<CryptoManagerFacade>();

            var result = target.KrypterUtenBrukerinnsyn(new byte[] {1});

            result.Should().Equal(0x9C, 0x58, 0xE3, 0x3A, 0xD6, 0xA9, 0x3B, 0xD1, 0xCB, 0xD2, 0x3D, 0x2A, 0xF7, 0x03, 0x51, 0x59);
        }


        [Fact]
        public void DekrypterUtenBrukerinnsyn_GittBytes_ReturnererKryptertBytes()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<CryptoManagerFacade.Konfig>, CryptoManagerFacade.Konfig>(x => x.Value)
                .Returns(new CryptoManagerFacade.Konfig
                {
                    UseEmbeddedTestKey = true
                });

            var target = automocker.CreateInstance<CryptoManagerFacade>();

            var result = target.DekrypterUtenBrukerinnsyn(new byte[] { 0x9C, 0x58, 0xE3, 0x3A, 0xD6, 0xA9, 0x3B, 0xD1, 0xCB, 0xD2, 0x3D, 0x2A, 0xF7, 0x03, 0x51, 0x59 });

            result.Should().Equal(1);
        }


        [Fact]
        public void DekrypterForBruker_GittKryptertTekstOgBrukerinfo_ReturnererDekryptertVerdiOgLoggerInnsyn()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<CryptoManagerFacade.Konfig>, CryptoManagerFacade.Konfig>(x => x.Value)
                .Returns(new CryptoManagerFacade.Konfig
                {
                    UseEmbeddedTestKey = true
                });

            var target = automocker.CreateInstance<CryptoManagerFacade>();

            var result = target.DekrypterForBruker("79E2A544964802DE4B2CC345D784A8FB", "Testfelt", "For testing", "Ola");

            result.Should().Be("test");

            automocker.Verify<IInnsynloggRespository>(x => x.OpprettOgLagre(It.Is<Innsynlogg>(i => 
                i.Felt == "Testfelt" &&
                i.Hvem == "Ola" &&
                i.Hvorfor == "For testing" &&
                i.Hva == "79E2A544964802DE4B2CC345D784A8FB")), Times.Once);
        }

        [Fact]
        public void KrypterForBruker_GittKlartekstOgBrukerinfo_ReturnererKryptertTekstVerdiOgLoggerInnsyn()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<CryptoManagerFacade.Konfig>, CryptoManagerFacade.Konfig>(x => x.Value)
                .Returns(new CryptoManagerFacade.Konfig
                {
                    UseEmbeddedTestKey = true
                });

            var target = automocker.CreateInstance<CryptoManagerFacade>();

            var result = target.KrypterForBruker("test", "Testfelt", "For testing", "Ola");

            result.Should().Be("79E2A544964802DE4B2CC345D784A8FB");

            automocker.Verify<IInnsynloggRespository>(x => x.OpprettOgLagre(It.Is<Innsynlogg>(i =>
                i.Felt == "Testfelt" &&
                i.Hvem == "Ola" &&
                i.Hvorfor == "For testing" &&
                i.Hva == "79E2A544964802DE4B2CC345D784A8FB")), Times.Once);
        }

        [Fact]
        public void DekrypterDataTilknyttet_GittKryptertTekstOgBrukerinfo_ReturnererDekryptertTekstOgLoggerInnsyn()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<CryptoManagerFacade.Konfig>, CryptoManagerFacade.Konfig>(x => x.Value)
                .Returns(new CryptoManagerFacade.Konfig
                {
                    UseEmbeddedTestKey = true
                });

            var target = automocker.CreateInstance<CryptoManagerFacade>();

            var result = target.DekrypterDataTilknyttet("79E2A544964802DE4B2CC345D784A8FB", "<kryptert-tilknyttet>", "Testfelt", "For testing", "Ola");

            result.Should().Be("test");

            automocker.Verify<IInnsynloggRespository>(x => x.OpprettOgLagre(It.Is<Innsynlogg>(i =>
                i.Felt == "Testfelt" &&
                i.Hvem == "Ola" &&
                i.Hvorfor == "For testing" &&
                i.Hva == "<kryptert-tilknyttet>")), Times.Once);
        }

        [Fact]
        public void DekrypterDataTilknyttet_GittKryptertBytesOgBrukerinfo_ReturnererDekryptertBytesOgLoggerInnsyn()
        {
            var automocker = new AutoMocker();

            automocker.Setup<IOptions<CryptoManagerFacade.Konfig>, CryptoManagerFacade.Konfig>(x => x.Value)
                .Returns(new CryptoManagerFacade.Konfig
                {
                    UseEmbeddedTestKey = true
                });

            var target = automocker.CreateInstance<CryptoManagerFacade>();

            var result = target.DekrypterDataTilknyttet(new byte[] { 0x9C, 0x58, 0xE3, 0x3A, 0xD6, 0xA9, 0x3B, 0xD1, 0xCB, 0xD2, 0x3D, 0x2A, 0xF7, 0x03, 0x51, 0x59 }, "<kryptert-tilknyttet>", "Testfelt", "For testing", "Ola");

            result.Should().Equal(1);

            automocker.Verify<IInnsynloggRespository>(x => x.OpprettOgLagre(It.Is<Innsynlogg>(i =>
                i.Felt == "Testfelt" &&
                i.Hvem == "Ola" &&
                i.Hvorfor == "For testing" &&
                i.Hva == "<kryptert-tilknyttet>")), Times.Once);
        }
    }
}