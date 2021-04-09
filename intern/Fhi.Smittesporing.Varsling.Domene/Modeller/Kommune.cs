using System;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public class Kommune : IOpprettetAv, ISistOppdatertAv
    {
        public int KommuneId { get; set; }
        public string Navn { get; set; }
        public string KommuneNr { get; set; }
        public string SmsFletteinfo { get; set; }

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }
    }
}