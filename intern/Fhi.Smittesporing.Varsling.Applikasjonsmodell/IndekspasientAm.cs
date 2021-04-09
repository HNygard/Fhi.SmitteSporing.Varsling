using System;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell
{
    public class IndekspasientAm
    {
        public int IndekspasientId { get; set; }
        public int? KommuneId { get; set; }
        public string Status { get; set; }
        public string Varslingsstatus { get; set; }
        public bool KanGodkjennesForVarsling { get; set; }
        public DateTime Opprettettidspunkt { get; set; }
        public DateTime? Provedato { get; set; }

        public class Filter
        {
            public bool? ErFerdig { get; set; }
            public bool? KreverGodkjenning { get; set; }
            public bool? ErRegistert { get; set; }
            public bool? MedSmittekontakt { get; set; }
            public bool? ManglerKontaktinfo { get; set; }
            public string KommuneNr { get; set; }
            public DateTime? FraOgMed { get; set; }
            public DateTime? TilOgMed { get; set; }
            public string Telefonnummer { get; set; }

            public int? Sideindeks { get; set; }
            public int? Sideantall { get; set; }
        }
    }

    public class IndekspasientMedAntallAm : IndekspasientAm
    {
        public int AntallSmittekontakter { get; set; }
    }

    public class IndekspasientPersonopplysningerAm
    {
        public string Fodselsnummer { get; set; }
        public string Telefonnummer { get; set; }
    }
}
