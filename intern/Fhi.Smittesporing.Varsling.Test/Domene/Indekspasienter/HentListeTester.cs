using System;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Moq;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.Indekspasienter
{
    public class HentListeTester : DomeneTestBase
    {
        [Fact]
        [Trait("Indekspasient", "HentListe")]

        public void Test_TomtFilter()
        {
            InsertSmitteTilfelleHelper("12345678901", "12345678",(s)=> { });
            var handler = new HentListe.Handler(IndekspasientRepository, Mapper, new Mock<ITelefonNormalFacade>().Object, new Mock<ICryptoManagerFacade>().Object);

            var request = new HentListe.Query
            {
                Filter = new IndekspasientAm.Filter()
            };

            //act
            Task<PagedListAm<IndekspasientMedAntallAm>> task = handler.Handle(request, CancellationToken);
            //assert
            PagedListAm<IndekspasientMedAntallAm> pagedList = task.Result;
            Assert.Equal(1, pagedList.TotaltAntall);
        }

        [Theory]
        [Trait("Indekspasient", "HentListe")]
        [InlineData(Varsling.Domene.Modeller.Varslingsstatus.TilGodkjenning, true, 1)]
        [InlineData(Varsling.Domene.Modeller.Varslingsstatus.TilGodkjenning, false, 0)]

        public void Test_Filter_KreverGodkjenning(Varsling.Domene.Modeller.Varslingsstatus varslingsstatus,bool filtervalue,int antall)
        {
            InsertSmitteTilfelleHelper("12345678901", "12345678", (smitteTilfelle) => smitteTilfelle.Varslingsstatus= varslingsstatus);
            var handler = new HentListe.Handler(IndekspasientRepository, Mapper, new Mock<ITelefonNormalFacade>().Object, new Mock<ICryptoManagerFacade>().Object);
            var request = new HentListe.Query
            {
                Filter = new IndekspasientAm.Filter
                {
                    KreverGodkjenning = filtervalue
                }
            };

            //act
            Task<PagedListAm<IndekspasientMedAntallAm>> task = handler.Handle(request, CancellationToken);
            //assert
            PagedListAm<IndekspasientMedAntallAm> pagedList = task.Result;
            Assert.Equal(antall, pagedList.TotaltAntall);
        }


        private void InsertSmitteTilfelleHelper(string fodselnummer, string telefonnummer ,Action<Varsling.Domene.Modeller.Indekspasient> action  )
        {
            var smitteTilfelle = new Varsling.Domene.Modeller.Indekspasient
            {
                Status = Varsling.Domene.Modeller.IndekspasientStatus.SmitteKontakt,
                Fodselsnummer = fodselnummer,
                Telefon = string.IsNullOrEmpty(telefonnummer)
                    ? null
                    : new Varsling.Domene.Modeller.Telefon
                    {
                        Telefonnummer = telefonnummer
                    }
            };
            action(smitteTilfelle);

            DbContext.Add(smitteTilfelle);
            DbContext.SaveChanges();
        }

        



    }
}