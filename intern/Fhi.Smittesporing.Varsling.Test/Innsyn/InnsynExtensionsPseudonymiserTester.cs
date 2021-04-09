using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Domene.InnsynsLogg;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Innsyn
{
    public class InnsynExtensionsPseudonymiserTester
    {
        [Fact]
        public void TaklerNull()
        {
            IEnumerable<InnsynLoggHelsenorgeAm> input = null;

            var actual = input.Pseudonymiser();

            Assert.Null(actual);
        }

        [Fact]
        public void PseudonymisererEttElement()
        {
            var dato = DateTime.Now;
            var formal = "Rai rai";
            var navn = @"fhi\anau";

            var input = new[]
            {
                Create(dato, formal, navn)
            };

            var actual = input.Pseudonymiser();

            Assert.NotNull(actual);
            Assert.Single(actual);
            Assert.Equal("Smittejeger 1", actual.Single().Navn, true);
            Assert.Equal("FHI", actual.Single().Organisasjon);
        }

        [Fact]
        public void PseudonymisererFlereElement()
        {
            var dato = DateTime.Now;
            var formal = "Rai rai";
            var navn = @"fhi\anau";

            var input = new[]
            {
                Create(dato, formal, navn),
                Create(dato, formal, navn)
            };

            var actual = input.Pseudonymiser();

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal("Smittejeger 1", actual.First().Navn, true);
            Assert.Equal("FHI", actual.First().Organisasjon);
            Assert.Equal("Smittejeger 1", actual.Last().Navn, true);
            Assert.Equal("FHI", actual.Last().Organisasjon);
        }

        [Fact]
        public void PseudonymisererForskjelligeElement()
        {
            var dato = DateTime.Now;
            var formal = "Rai rai";
            var navn = @"fhi\anau";
            var annetnavn = @"fhi\sibe";

            var input = new[]
            {
                Create(dato, formal, navn),
                Create(dato, formal, annetnavn),
                Create(dato, formal, navn),
                Create(dato, formal, annetnavn)
            };

            var actual = input.Pseudonymiser().ToArray();

            Assert.NotNull(actual);
            Assert.Equal(4, actual.Length);

            Assert.Equal("Smittejeger 1", actual[0].Navn, true);
            Assert.Equal("FHI", actual[0].Organisasjon);
            Assert.Equal("Smittejeger 2", actual[1].Navn, true);
            Assert.Equal("FHI", actual[1].Organisasjon);
            Assert.Equal("Smittejeger 1", actual[2].Navn, true);
            Assert.Equal("FHI", actual[2].Organisasjon);
            Assert.Equal("Smittejeger 2", actual[3].Navn, true);
            Assert.Equal("FHI", actual[3].Organisasjon);
        }

        [Fact]
        public void PseudonymisererHelsenorgebruker()
        {
            var dato = DateTime.Now;
            var formal = "Rai rai";

            var input = new[]
            {
                Create(dato, formal, InnsynExtensions.HELSENORGEBRUKER)
            };

            var actual = input.Pseudonymiser();

            Assert.NotNull(actual);
            Assert.Single(actual);
            Assert.Equal("Oppslag via Helsenorge.no", actual.Single().Formal); //TODO: Ikke hardkod strengen
        }

        [Fact]
        public void PseudonymisererHelsenorgebrukerOgAndreBrukere()
        {
            var dato = DateTime.Now;
            var formal = "Rai rai";
            var smittejeger1 = @"fhi\anau";
            var smittejeger2 = @"fhi\sibe";
            var person = "(18057832149) - Aanund Austrheim";

            var input = new[]
            {
                Create(dato, formal, smittejeger1),
                Create(dato, formal, InnsynExtensions.HELSENORGEBRUKER),
                Create(dato, formal, smittejeger2),
                Create(dato, InnsynExtensions.OPPSLAGVIAHELSENORGENO, person)
            };

            var actual = input.Pseudonymiser().ToArray();

            Assert.NotNull(actual);
            Assert.Equal(4, actual.Length);

            Assert.Equal("Smittejeger 1", actual[0].Navn, true);
            Assert.Equal("FHI", actual[0].Organisasjon);
            Assert.Equal("Systembruker", actual[1].Navn, true);
            Assert.Equal(InnsynExtensions.OPPSLAGVIAHELSENORGENO, actual[1].Formal, true);
            Assert.Equal("FHI", actual[1].Organisasjon);
            Assert.Equal("Smittejeger 2", actual[2].Navn, true);
            Assert.Equal("FHI", actual[2].Organisasjon);
            Assert.Equal("Aanund Austrheim", actual[3].Navn, true);
            Assert.Equal(string.Empty, actual[3].Organisasjon);
            Assert.Equal(InnsynExtensions.OPPSLAGVIAHELSENORGENO, actual[3].Formal, true);
        }


        //[Fact]
        //public void SetterOrganisasjonForSystemBrukere()
        //{
        //    var dato = DateTime.Now;
        //    var formal = "Rai rai";
        //    var navn = @"fhi\anau";
        //    var annetnavn = @"fhi\sibe";

        //    var input = new[]
        //    {
        //        Create(dato, formal, navn),
        //    };

        //    var expected = input.Pseudonymiser();

        //    Assert.Null(expected);
        //}

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
