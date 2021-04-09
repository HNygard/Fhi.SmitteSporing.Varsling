using System;
using System.Collections.Generic;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler;
using FluentAssertions;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Modeller.Varslingsregler
{
    public class SammeTelefonIkkeVarsletSammeDagRegelTester
    {
        [Fact]
        public void KanVarsles_GittIngenEksisterendeVarslerForTelefon_ErSant()
        {
            // Arrange
            var smittekontaktTelefonAldriVarslet = new Smittekontakt
            {
                Telefon = new Telefon(),
                SmsVarsler = new List<SmsVarsel>()
            };
            smittekontaktTelefonAldriVarslet.Telefon.SmittekontaktForTelefon = new List<Smittekontakt>
            {
                smittekontaktTelefonAldriVarslet
            };
            var target = new SammeTelefonIkkeVarsletSammeDagRegel();

            // Act
            var kanVarsles = target.KanVarsles(smittekontaktTelefonAldriVarslet);

            // Assert
            kanVarsles.Should().Be(true);
        }

        [Theory]
        [InlineData(SmsStatus.Sendt)]
        [InlineData(SmsStatus.Feilet)]
        [InlineData(SmsStatus.Levert)]
        [InlineData(SmsStatus.DelvisLevert)]
        [InlineData(SmsStatus.Klargjort)]
        [InlineData(SmsStatus.Opprettet)]
        [InlineData(SmsStatus.Ukjent)]
        public void KanVarsles_TidligereVarselFraForrigeDag_ErSant(SmsStatus varselStatus)
        {
            // Arrange
            var smittekontaktTelefonVarsletForrigeDag = new Smittekontakt
            {
                Telefon = new Telefon(),
                SmsVarsler = new List<SmsVarsel>()
            };
            smittekontaktTelefonVarsletForrigeDag.Telefon.SmittekontaktForTelefon = new List<Smittekontakt>
            {
                new Smittekontakt
                {
                    SmsVarsler = new List<SmsVarsel>
                    {
                        new SmsVarsel
                        {
                            Status = varselStatus,
                            Created = DateTime.Today.AddHours(-1)
                        }
                    }
                },
                smittekontaktTelefonVarsletForrigeDag
            };
            var target = new SammeTelefonIkkeVarsletSammeDagRegel();

            // Act
            var kanVarsles = target.KanVarsles(smittekontaktTelefonVarsletForrigeDag);

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
        public void KanVarsles_TidligereVarselFraSammeDag_ErUsant(SmsStatus varselStatus)
        {
            // Arrange
            var smittekontaktTelefonVarsletSammeDag = new Smittekontakt
            {
                Telefon = new Telefon(),
                SmsVarsler = new List<SmsVarsel>()
            };
            smittekontaktTelefonVarsletSammeDag.Telefon.SmittekontaktForTelefon = new List<Smittekontakt>
            {
                new Smittekontakt
                {
                    SmsVarsler = new List<SmsVarsel>
                    {
                        new SmsVarsel
                        {
                            Status = varselStatus,
                            Created = DateTime.Today.AddHours(1)
                        }
                    }
                },
                smittekontaktTelefonVarsletSammeDag
            };
            var target = new SammeTelefonIkkeVarsletSammeDagRegel();

            // Act
            var kanVarsles = target.KanVarsles(smittekontaktTelefonVarsletSammeDag);

            // Assert
            kanVarsles.Should().Be(false);
        }

        [Fact]
        public void KanVarsles_GittTidligereVarselFeilet_ErSant()
        {
            // Arrange
            var smittekontaktTelefonVarsletSammeDagMenFeilet = new Smittekontakt
            {
                Telefon = new Telefon(),
                SmsVarsler = new List<SmsVarsel>()
            };
            smittekontaktTelefonVarsletSammeDagMenFeilet.Telefon.SmittekontaktForTelefon = new List<Smittekontakt>
            {
                new Smittekontakt
                {
                    SmsVarsler = new List<SmsVarsel>
                    {
                        new SmsVarsel
                        {
                            Status = SmsStatus.Feilet,
                            Created = DateTime.Today.AddHours(1)
                        }
                    }
                },
                smittekontaktTelefonVarsletSammeDagMenFeilet
            };
            var target = new SammeTelefonIkkeVarsletSammeDagRegel();

            // Act
            var kanVarsles = target.KanVarsles(smittekontaktTelefonVarsletSammeDagMenFeilet);

            // Assert
            kanVarsles.Should().Be(true);
        }
    }
}