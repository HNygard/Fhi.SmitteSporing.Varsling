using System;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Rapport
{
    public class IndekspasientRapportAm
    {
        public DateTime Generertdato { get; set; }
        public PeriodeAm Periode { get; set; }
        public DateTime? SisteOpprettet { get; set; }
        public int AntallSisteDag { get; set; }
        public ChartDataAm ChartData { get; set; }

        public class Filter
        {
            public DateTime? FraOgMed { get; set; }
            public DateTime? TilOgMed { get; set; }
            public string KommuneNr { get; set; }
        }
    }
}
