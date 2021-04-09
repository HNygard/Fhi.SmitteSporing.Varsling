using System.Collections.Generic;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell
{
    public class VarslingssimuleringAm
    {
        public int IndekspasientId { get; set; }
        public int AntallKontakter { get; set; }
        public int AntallKontakterTilVarsling { get; set; }
        public int AntallKontakterUtenVarsling { get; set; }
        public IEnumerable<VarslingssimuleringdetaljerAm> Detaljer { get; set; }
    }

    public class VarslingssimuleringdetaljerAm
    {
        public int SmittekontaktId { get; set; }
        public bool KanVarsles { get; set; }
        public IEnumerable<VarslingsregelAm> VarselIkkeTillatAvRegler { get; set; }
        public IEnumerable<VarslingsregelAm> VarselTillatAvRegler { get; set; }
    }

    public class VarslingsregelAm
    {
        public string Navn { get; set; }
        public string Beskrivelse { get; set; }
    }
}