using System;
using System.Collections.Generic;

namespace Fhi.Smittesporing.Helsenorge.Api.Models
{
    public class InnsynHn
    {
        public string Telefonnummer { get; set; }
        public string Fodselsnummer { get; set; }

        public IEnumerable<DateTime> Prøvedatoer { get; set; }

        public IEnumerable<SmitteKontaktHn> Smittekontakter { get; set; }

        public IEnumerable<SmsVarselHn> SmsVarsel { get; set; }
    }
}
