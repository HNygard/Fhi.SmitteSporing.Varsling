using System;
using System.Collections.Generic;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using FluentAssertions;
using Moq;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Modeller
{
    public class SmsVarselTester
    {
        [Fact]
        public void LagUtsending_GittGyldigState_LagerUtsendingMedAltInnhold()
        {
            var crypto = new Mock<ICryptoManagerFacade>();
            crypto
                .Setup(x => x.DekrypterUtenBrukerinnsyn("kryptert-tlf"))
                .Returns("+4798765432");

            var utsendingRef = Guid.NewGuid();

            var target = new SmsVarsel
            {
                Smittekontakt = new Smittekontakt
                {
                    Telefon = new Telefon
                    {
                        Telefonnummer = "kryptert-tlf"
                    },
                    Detaljer = new List<SmittekontaktDetaljer>
                    {
                        new SmittekontaktDetaljer
                        {
                            Dato = DateTime.Today.AddDays(-2)
                        }
                    },
                    Indekspasient = new Indekspasient
                    {
                        Kommune = new Kommune
                        {
                            SmsFletteinfo = "Hello!"
                        }
                    },
                    Risikokategori = "low"
                },
                Status = SmsStatus.Opprettet,
                Referanse = utsendingRef
            };

            var resultat = target.LagUtsending(crypto.Object, new SmsFletteinnstillinger());

            resultat.Telefonnummer.Should().Be("+4798765432");
            resultat.Referanse.Should().Be(utsendingRef);
            resultat.Flettedata.Should().ContainKey("kontaktDato");
            resultat.Flettedata.Should().ContainKey("kommuneInfo");
            resultat.Flettedata.Should().ContainKey("risikoTekst");
        }
    }
}