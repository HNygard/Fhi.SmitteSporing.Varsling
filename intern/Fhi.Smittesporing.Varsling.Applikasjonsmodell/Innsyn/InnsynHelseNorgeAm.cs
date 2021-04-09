using System;
using System.Collections.Generic;

namespace Fhi.Smittesporing.Varsling.Intern.Applikasjonsmodell
{
    public class InnsynHelsenorgeAm
    {
        public string Telefonnummer { get; set; }
        public string Fodselsnummer { get; set; }
        public IEnumerable<DateTime> Prøvedatoer { get; set; }
        public IEnumerable<InnsynHelsenorgeSmittekontaktAm> Smittekontakter { get; set; }
        public IEnumerable<InnsynHelsenorgeSmsvarselAm> SmsVarsel { get; set; }
    }

    public class InnsynHelsenorgeSmittekontaktAm
    {
        public DateTime Dato { get; set; }
        public DateTime? Varslet { get; set; }
        public string Risikokategori { get; set; }
        public string Verifiseringskode { get; set; }
    }

    public class InnsynHelsenorgeSmsvarselAm
    {
        public DateTime Tidspunkt { get; set; }
        public string Status { get; set; }
        public string Kode { get; set; }
        public string Verifiseringskode { get; set; }
    }
}
