using System;
using System.Linq;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler
{
    public class SammeTelefonIkkeVarsletSammeDagRegel : IVarslingsregel
    {
        public bool KanVarsles(Smittekontakt kontakt)
        {
            var smittekontaktForTelefon = kontakt.Telefon.SmittekontaktForTelefon ??
                throw new Exception("Regel krever at all smittekontakt for tilhørende telefon er tilgjengelig.");
            var erVarsletSammeDag = smittekontaktForTelefon.Any(k => 
                k.SmsVarsler?.Any(v => v.Status != SmsStatus.Feilet && v.Created.Date == DateTime.Today) ??
                throw new Exception("Regel krever at alle tidliger varsler for telefon er tilgjengelig."));

            return !erVarsletSammeDag;
        }

        public string Navn => "Ikke flere varsler samme telefon samme dag";

        public string Beskrivelse =>
            "Smittekontakter for telefon som har fått varsel samme dag skal ikke få nye varsler";
    }
}