using System;
using System.Collections.Generic;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public class Telefon : IOpprettetAv, ISistOppdatertAv
    {
        public int TelefonId { get; set; }
        [AuditTrailOptions(SensitivVerdi = true)]
        public string Telefonnummer { get; set; }

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }

        // Navigasjonsegenskaper
        public ICollection<Smittekontakt> SmittekontaktForTelefon { get; set; }
        public ICollection<Indekspasient> IndekspasienterForTelefon { get; set; }
    }
}
