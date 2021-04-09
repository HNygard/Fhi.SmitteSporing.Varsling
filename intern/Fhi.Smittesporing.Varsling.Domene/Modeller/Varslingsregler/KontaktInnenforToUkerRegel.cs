using System;
using System.Linq;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler
{
    /// <summary>
    /// Kontakt med smittet person må ha vært innenfor de to siste ukene for å gi varsling
    /// </summary>
    public class KontaktInnenforToUkerRegel : IVarslingsregel
    {
        public bool KanVarsles(Smittekontakt kontakt)
        {
            var eldsteKontaktdatoForVarsling = DateTime.Today.AddDays(-14);
            return kontakt.Detaljer.Any(d => d.Dato >= eldsteKontaktdatoForVarsling);
        }

        public string Navn => "Kun varsle kontakt innenfor 2 uker";
        public string Beskrivelse => "Siste kontakt må ha vært innenfor 2 uker for at varsel skal gå ut.";
    }
}