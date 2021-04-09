using System;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn
{
    public class InnsynIndekspasientAm
    {
        public DateTime Opprettettidspunkt { get; set; }
        public DateTime? Provedato { get; set; }
        public string Kommune { get; set; }
        public DateTime Created { get; set; }

        public string Status { get; set; }
        public string Varslingsstatus { get; set; }
        public bool KanGodkjennesForVarsling { get; set; }
    }
}
