using System;
using System.Collections.Generic;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Msis;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Varsling.Intern.MockControllers
{
    [ApiController]
    [Route("mockapi/[controller]")]
    [ApiExplorerSettings(GroupName = "mocks")]
    public class MsisMockController : ControllerBase
    {
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        private static DateTime _opprettettidspunkt;
        private static bool _firstCall = true;

        private readonly List<Kommune> _kommune = new List<Kommune>
        {
            new Kommune {Navn = null, KommuneNr = null},
            new Kommune {Navn = null, KommuneNr = null},
            new Kommune {Navn = "Bergen", KommuneNr = "4601"},
            new Kommune {Navn = "Bjørnafjorden", KommuneNr = "4624"},
            new Kommune {Navn = "Oslo", KommuneNr = "0301"},
            new Kommune {Navn = "Averøy", KommuneNr = "1554"},
            new Kommune {Navn = "Notodden", KommuneNr = "0807"}
        };

        [HttpGet("api/smittesporing/korona")]
        public ActionResult<IEnumerable<MsisSmittetilfelle>> Hent([FromQuery]DateTime fraTidspunkt)
        {

            if (_firstCall)
            {
                _opprettettidspunkt = fraTidspunkt;
                _firstCall = false;
            }

            if (_opprettettidspunkt< DateTime.Now.AddHours(-10))
            {
                _opprettettidspunkt = _opprettettidspunkt.AddHours(1);
            }





            List<MsisSmittetilfelle> liste = new List<MsisSmittetilfelle>();

            for(int i=0;i< 1+Rand.Next(3);i++)
            {
                var kommune = Rand.NextDouble() > 0.25
                    ? _kommune[Rand.Next(0, _kommune.Count)]
                    : null;

                MsisSmittetilfelle msisSmittetilfelle = new MsisSmittetilfelle
                {
                    Fodselsnummer = (Rand.Next(4) % 3==1)?"":"test" + Rand.Next(100000000, 320000000).ToString(),
                    Opprettettidspunkt = _opprettettidspunkt,
                    Provedato = (Rand.Next(4) % 3 == 1) ? (DateTime?)null:_opprettettidspunkt.AddSeconds(-Rand.Next(100)),
                    Bostedkommune= kommune?.Navn,
                    Bostedkommunenummer = kommune?.KommuneNr,
                };
                _opprettettidspunkt=_opprettettidspunkt.AddSeconds(1);
                liste.Add(msisSmittetilfelle);
            }

            return Ok(liste);
        } // HentPersoner

    }

}
