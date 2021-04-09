using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Domene.InnsynsLogg;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace Fhi.Smittesporing.Varsling.Test.Innsyn
{
    public class InnsynExtensionsReduserTester
    {
        [Fact]
        public void KasterIkkeNullreferenceException()
        {
            IEnumerable<InnsynLoggHelsenorgeAm> input = null;

            var expected = input.Reduce();

            Assert.Null(expected);
        }

        [Fact]
        public void ReduserSlårSammenDeToFørste()
        {
            var dato = DateTime.Now;
            var formal = InnsynExtensions.OPPSLAGVIAHELSENORGENO;
            var navn = "Aanund Austrheim";
            var annetnavn = "Sindre";
            var organisasjon = "FHI";

            var input = new[]
            {
                Create(dato, formal, navn, organisasjon),
                Create(dato, formal, navn, organisasjon),
                Create(dato, formal, annetnavn, organisasjon)
            };

            var actual = input.Reduce();

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal(navn, actual.First().Navn);
            Assert.Equal(annetnavn, actual.Last().Navn);
        }

        [Fact]
        public void ReduserSlårIkkeSammenLikeMedGapImellom()
        {
            var dato = DateTime.Now;
            var formal = InnsynExtensions.OPPSLAGVIAHELSENORGENO;
            var navn = "Aanund Austrheim";
            var annetnavn = "Sindre";
            var organisasjon = "FHI";

            var input = new[]
            {
                Create(dato, formal, navn, organisasjon),
                Create(dato, formal, annetnavn, organisasjon),
                Create(dato, formal, navn, organisasjon)
            };

            var actual = input.Reduce().ToArray();

            Assert.NotNull(actual);
            Assert.Equal(3, actual.Length);
            Assert.Equal(navn, actual[0].Navn);
            Assert.Equal(annetnavn, actual[1].Navn);
            Assert.Equal(navn, actual[2].Navn);
        }

        [Fact]
        public void RedusererMangeTilEn()
        {
            var dato = DateTime.Now;
            var formal = InnsynExtensions.OPPSLAGVIAHELSENORGENO;
            var navn = "Aanund Austrheim";
            var organisasjon = "FHI";

            var input = new[]
            {
                Create(dato, formal, navn, organisasjon),
                Create(dato, formal, navn, organisasjon),
                Create(dato, formal, navn, organisasjon)
            };

            var actual = input.Reduce();

            Assert.NotNull(actual);
            Assert.Single(actual);
        }

        [Fact]
        public void RedusererMangeTilEnHvisDatoErInnenDelta()
        {
            var dato = DateTime.Now;
            var formal = InnsynExtensions.OPPSLAGVIAHELSENORGENO;
            var navn = "Aanund Austrheim";
            var organisasjon = "FHI";

            var input = new[]
            {
                Create(dato.AddSeconds(-(60 * 5 - 1)), formal, navn, organisasjon),
                Create(dato, formal, navn, organisasjon),
                Create(dato.AddSeconds(60 * 5 - 1), formal, navn, organisasjon)
            };

            var actual = input.Reduce();

            Assert.NotNull(actual);
            Assert.Single(actual);
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
