using System;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn
{
    public class InnsynSmsVarselAm
	{
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }
        public DateTime? SisteEksterneHendelsestidspunkt { get; set; }
        public Guid? Referanse { get; set; }
        public string Verifiseringskode { get; set; }
    }
}
