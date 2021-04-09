using System;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    /// <summary>
    /// Egen modell for diagramfil for å kunne effektivt endre andre data i smittekontakter
    /// </summary>
    public class SmittekontaktGpsHistogram : IOpprettetAv, ISistOppdatertAv
    {
        public int SmittekontaktGpsHistogramId { get; set; }
        public int SmittekontaktId { get; set; }
        [AuditTrailOptions(BinarVerdi = true)]
        public byte[] Data { get; set; }

        // EF Navigasjonsegenskaper
        public Smittekontakt Smittekontakt { get; set; }

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }
    }
}