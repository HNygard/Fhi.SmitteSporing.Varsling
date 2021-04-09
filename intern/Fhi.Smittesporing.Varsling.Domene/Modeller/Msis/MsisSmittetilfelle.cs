using System;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Msis
{
    public class MsisSmittetilfelle
    {
        public string Fodselsnummer { get;  set; }
        public DateTime Opprettettidspunkt { get;  set; }
        public DateTime? Provedato { get; set; }
        public string Bostedkommunenummer { get; set; }
        public string Bostedkommune { get; set; }
    }

}
