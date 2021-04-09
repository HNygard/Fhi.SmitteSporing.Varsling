using System;
using System.Linq;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler
{
    public class SammeKontaktIkkeVarsletRegel : IVarslingsregel
    {
        public bool KanVarsles(Smittekontakt kontakt)
        {
            var kontaktErVarslet = kontakt.SmsVarsler?.Any(x => x.Status != SmsStatus.Feilet) ??
                                   throw new Exception("Regler krever at eksisterende SmsVarsler er tilgjengelig");

            return !kontaktErVarslet;
        }

        public string Navn => "Kun ett varsel per kontakt";
        public string Beskrivelse => "Det skal ikke sendes mer enn ett varsel per smittekontakt";
    }
}