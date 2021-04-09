using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Domene.InnsynsLogg;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Innsyn
{
    public class InnsynExtensionsEkskluderInnbyggerTester
    {
        [Fact]
        public void EkskluderInnbyggerKasterIkkeNullReferenceException()
        {
            IEnumerable<InnsynLoggHelsenorgeAm> input = null;

            var expected = input.EkskluderInnbygger(new InnsynFilterAm
            {
                Fodselsnummer = "11111111111",
                Telefonnummer = "90729920"
            });

            Assert.Null(expected);
        }

        [Fact]
        public void EkskluderInnbyggerFjernerInnbygger()
        {
            var dato = DateTime.Now;
            var formal = InnsynExtensions.OPPSLAGVIAHELSENORGENO;
            var navn = "(11111111111) - Aanund Austrheim";
            var organisasjon = string.Empty;

            var input = new[]
            {
                Create(dato, formal, navn, organisasjon),
                Create(dato, formal, navn, organisasjon)
            };

            var actual = input.EkskluderInnbygger(new InnsynFilterAm
            {
                Fodselsnummer = "11111111111",
                Telefonnummer = "11111111"
            });

            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void EkskluderInnbyggerFjernerBareInnbygger()
        {
            var dato = DateTime.Now;
            var formal = InnsynExtensions.OPPSLAGVIAHELSENORGENO;
            var navn = "(11111111111) - Aanund Austrheim";
            var organisasjon = string.Empty;
            var annetnavn = "(22222222222) - Sindre";

            var input = new[]
            {
                Create(dato, formal, navn, organisasjon),
                Create(dato, formal, annetnavn, organisasjon),
                Create(dato, formal, navn, organisasjon)
            };

            var actual = input.EkskluderInnbygger(new InnsynFilterAm
            {
                Fodselsnummer = "11111111111",
                Telefonnummer = "11111111"
            });

            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            var single = actual.Single();
            Assert.Equal(annetnavn, single.Navn);
        }

        private InnsynLoggHelsenorgeAm Create(DateTime dato, string formal, string navn, string organisasjon = "")
        {
            return new InnsynLoggHelsenorgeAm
            {
                Dato = dato,
                Formal = formal,
                Navn = navn,
                Organisasjon = organisasjon
            };
        }
    }
}
