using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Msis;
using Moq;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test.Domene.Indekspasienter
{
    public class HentFraMsisTester : DomeneTestBase
    {
        private readonly Mock<IMsisFacade> _msisFacade;
  
        public HentFraMsisTester()
        {
            var list = new List<MsisSmittetilfelle>
            {
                new MsisSmittetilfelle {Opprettettidspunkt = DateTime.Parse("2020-04-02 12:38:11.1468217")},
                new MsisSmittetilfelle {Opprettettidspunkt = DateTime.Parse("2020-04-03 12:38:11.1468217")},
                new MsisSmittetilfelle {Opprettettidspunkt = DateTime.Parse("2020-04-04 12:38:11.1468217")}
            };

            _msisFacade = new Mock<IMsisFacade>();
            _msisFacade
                .Setup(m => m.GetSmittetilfeller(It.IsAny<DateTime>()))
                .ReturnsAsync((DateTime fraDato) =>
                {
                    return list.Where(e => e.Opprettettidspunkt> fraDato);
                });
        }

        [Theory]
        [Trait("Indekspasient", "HentFraMsis")]
        [InlineData("2020-04-02 12:00:00", 3)]
        [InlineData("2020-04-03 12:00:00", 2)]
        [InlineData("2020-04-04 12:00:00", 1)]
        [InlineData("2020-04-05 12:00:00", 0)]
        public async Task TestReturnerForventetAntall(string date, int expectedCount )
        {
            //arrange
            var handler = new HentFraMsis.Handler(_msisFacade.Object);
            Assert.NotNull(handler);

            var request = new HentFraMsis.Query
            {
                FraDato= DateTime.Parse(date)
            };

            //act
            var liste = await handler.Handle(request, CancellationToken);

            //assert
            Assert.Equal(expectedCount, liste.Count());
        }
    }
}