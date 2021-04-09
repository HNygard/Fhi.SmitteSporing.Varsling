using System.Collections.Generic;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler;
using FluentAssertions;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Modeller.Varslingsregler
{
    public class ErIkkeSmittetSelvRegelTester
    {
        [Fact]
        public void KanVarsles_GittTelefonIkkeKnyttetTilSmitte_ErSant()
        {
            // Arrange
            var smittekontaktIkkeSelvSmittet = new Smittekontakt
            {
                Telefon = new Telefon
                {
                    IndekspasienterForTelefon = new List<Indekspasient>()
                }
            };
            var target = new ErIkkeSmittetSelvRegel();

            // Act
            var kanVarsles = target.KanVarsles(smittekontaktIkkeSelvSmittet);

            // Assert
            kanVarsles.Should().Be(true);
        }

        [Fact]
        public void KanVarsles_GittTelefonKnyttetTilSmitte_ErUsant()
        {
            // Arrange
            var smittekontaktErSelvSmittet = new Smittekontakt
            {
                Telefon = new Telefon
                {
                    IndekspasienterForTelefon = new List<Indekspasient>
                    {
                        new Indekspasient()
                    }
                }
            };
            var target = new ErIkkeSmittetSelvRegel();

            // Act
            var kanVarsles = target.KanVarsles(smittekontaktErSelvSmittet);

            // Assert
            kanVarsles.Should().Be(false);
        }
    }
}