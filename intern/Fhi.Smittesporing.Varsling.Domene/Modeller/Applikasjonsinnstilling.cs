using System;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public class Applikasjonsinnstilling : IOpprettetAv, ISistOppdatertAv
    {
        public int ApplikasjonsinnstillingId { get; set; }
        public string Nokkel { get; set; }
        public string Verdi { get; set; }

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }
    }
}