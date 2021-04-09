using System;
using System.Linq;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler
{
    public class ErIkkeSmittetSelvRegel : IVarslingsregel
    {
        public bool KanVarsles(Smittekontakt kontakt)
        {
            // Hvis telefon er knyttet til pasient med koronasmitte regnes bruker av telefon som smittet
            var erSelvSmittet = kontakt.Telefon.IndekspasienterForTelefon?.Any() ??
                                throw new Exception("Regel krever at pasienter for tilknyttet telefon er tilgjengelig.");

            return !erSelvSmittet;
        }

        public string Navn => "Ikke varsle en som selv er smittet";
        public string Beskrivelse =>
            "Smittekontakter for telefon som selv er knyttet til koronasmitte skal ikke få varsler";
    }
}