using System;
using System.Collections.Generic;

namespace Fhi.Smittesporing.Simula.Applikasjonsmodell
{
    public class SimulaKontakt
    {
        public string Verifiseringskode { get; set; }
        public string Telefonnummer { get; set; }
        public SimulaKontaktOppsummering Oppsummering { get; set; }
        public SimulaKontaktdetaljer[] Detaljer { get; set; }
        public SimulaKontaktVersjonsinfo Versjonsinfo { get; set; }
    }

    public class SimulaKontaktVersjonsinfo
    {
        public string Pipeline { get; set; }
        public List<string> Enhet { get; set; }
    }

    public class SimulaKontaktOppsummering
    {
        public All AllKontakt { get; set; }
        public BluetoothInfo BluetoothKontakt { get; set; }
        public GpsInfo GpsKontakt { get; set; }

        public class All
        {
            public string Risikokategori { get; set; }
            public string SoylePlotBase64Png { get; set; }
            public int AntallKontakter { get; set; }
            public int AntallDagerMedKontakt { get; set; }
            public Dictionary<string, double> Interessepunkter { get; set; }
        }

        public class PerType
        {
            public double AkkumulertVarighet { get; set; }
            public double AkkumulertRisikoscore { get; set; }
            public int AntallDagerMedKontakt { get; set; }
        }

        public class GpsInfo : PerType
        {
            public string HistogramBase64Png { get; set; }
        }

        public class BluetoothInfo : PerType
        {
            public double VeldigNarVarighet { get; set; }
            public double RelativtNarVarighet { get; set; }
            public double NarVarighet { get; set; }
        }
    }

    public class SimulaKontaktdetaljer
    {
        public DateTime Dato { get; set; }
        public All AllKontakt { get; set; }
        public BluetoothInfo BluetoothKontakt { get; set; }
        public PerType GpsKontakt { get; set; }

        public class All
        {
            public string OppsummertPlotHtml { get; set; }

            public Dictionary<string, double> Interessepunkter { get; set; }
        }

        public class PerType
        {
            public double AkkumulertVarighet { get; set; }

            public double AkkumulertRisiko { get; set; }

            public double? Medianavstand { get; set; }
        }

        public class BluetoothInfo : PerType
        {
            public double VeldigNarVarighet { get; set; }
            public double RelativtNarVarighet { get; set; }
            public double NarVarighet { get; set; }
        }
    }
}