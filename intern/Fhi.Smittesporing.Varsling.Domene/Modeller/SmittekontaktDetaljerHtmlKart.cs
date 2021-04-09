using System;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    /// <summary>
    /// Egen modell for store tekstbolker for å kunne effektivt endre andre data i modeller
    /// </summary>
    public class SmittekontaktDetaljerHtmlKart : IOpprettetAv, ISistOppdatertAv
    {
        public int SmittekontaktDetaljerHtmlKartId { get; set; }
        public int SmittekontaktDetaljerId { get; set; }
        [AuditTrailOptions(BinarVerdi = true)]
        public string Innhold { get; set; }

        // EF Navigation
        public SmittekontaktDetaljer SmittekontaktDetaljer { get; set; }

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }
    }
}