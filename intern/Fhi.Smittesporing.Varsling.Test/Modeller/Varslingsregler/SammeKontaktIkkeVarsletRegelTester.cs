using System.Collections.Generic;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler;
using FluentAssertions;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Modeller.Varslingsregler
{
    public class SammeKontaktIkkeVarsletRegelTester
    {
        [Fact]
        public void KanVarsles_GittIngenEksisterendeVarsler_ErSant()
        {
            // Arrange
            var smittekontaktIkkeVarslet = new Smittekontakt
            {
                SmsVarsler = new List<SmsVarsel>()
            };
            var target = new SammeKontaktIkkeVarsletRegel();

            // Act
            var kanVarsles = target.KanVarsles(smittekontaktIkkeVarslet);

            // Assert
            kanVarsles.Should().Be(true);
        }

        [Fact]
        public void KanVarsles_GittAlleTidligereVarslerFeilet_ErSant()
        {
            // Arrange
            var smittekontaktVarslerFeilet = new Smittekontakt
            {
                SmsVarsler = new List<SmsVarsel>
                {
                    new SmsVarsel { Status = SmsStatus.Feilet },
                    new SmsVarsel { Status = SmsStatus.Feilet }
                }
            };
            var target = new SammeKontaktIkkeVarsletRegel();

            // Act
            var kanVarsles = target.KanVarsles(smittekontaktVarslerFeilet);

            // Assert
            kanVarsles.Should().Be(true);
        }

        [Theory]
        [InlineData(SmsStatus.Sendt)]
        [InlineData(SmsStatus.Levert)]
        [InlineData(SmsStatus.DelvisLevert)]
        [InlineData(SmsStatus.Klargjort)]
        [InlineData(SmsStatus.Opprettet)]
        [InlineData(SmsStatus.Ukjent)]
        public void KanVarsles_GittTidligereVarselIkkeFeilet_ErUsant(SmsStatus varselStatus)
        {
            // Arrange
            var smittekontaktVarselIkkeFeilet = new Smittekontakt
            {
                SmsVarsler = new List<SmsVarsel>
                {
                    new SmsVarsel { Status = varselStatus }
                }
            };
            var target = new SammeKontaktIkkeVarsletRegel();

            // Act
            var kanVarsles = target.KanVarsles(smittekontaktVarselIkkeFeilet);

            // Assert
            kanVarsles.Should().Be(false);
        }
    }
}