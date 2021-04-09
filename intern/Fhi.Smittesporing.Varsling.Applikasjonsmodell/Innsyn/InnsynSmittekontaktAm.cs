using System;
using System.Collections.Generic;
using System.Linq;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn
{
    public class InnsynSmittekontaktAm
    {
        public DateTime? VarsletTidspunkt { get; set; }

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }

        public string Risikokategori { get; set; }
        public int AntallKontakter { get; set; }
        public string InteressepunkterJson { get; set; }

        public double BluetoothAkkumulertVarighet { get; set; }
        public double BluetoothAkkumulertRisikoscore { get; set; }

        public double GpsAkkumulertVarighet { get; set; }
        public double GpsAkkumulertRisikoscore { get; set; }
        public string Verifiseringskode { get; set; }
    }
}
