using System;
using System.Collections.Generic;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell
{
    public class SmittekontaktListemodellAm
    {
        public int SmittekontaktId { get; set; }
        public int IndekspasientId { get; set; }
        public string Risikokategori { get; set; }
        public double BluetoothAkkumulertVarighet { get; set; }
        public double GpsAkkumulertVarighet { get; set; }
        public int AntallKontakter { get; set; }
        public Dictionary<string, double> Interessepunkter { get; set; }
        public DateTime? SisteKontaktDato { get; set; }
        public DateTime? VarsletTidspunkt { get; set; }
    }

    public class SmittekontaktAm
    {
        public int SmittekontaktId { get; set; }
        public int IndekspasientId { get; set; }
        public DateTime? VarsletTidspunkt { get; set; }

        public string Risikokategori { get; set; }
        public int AntallKontakter { get; set; }
        public int? AntallDagerMedKontakt { get; set; }
        public Dictionary<string, double> Interessepunkter { get; set; }

        public double BluetoothAkkumulertVarighet { get; set; }
        public double BluetoothAkkumulertRisikoscore { get; set; }
        public double BluetoothVeldigNarVarighet { get; set; }
        public double BluetoothRelativtNarVarighet { get; set; }
        public double BluetoothNarVarighet { get; set; }
        public int? BluetoothAntallDagerMedKontakt { get; set; }

        public double GpsAkkumulertVarighet { get; set; }
        public double GpsAkkumulertRisikoscore { get; set; }
        public int? GpsAntallDagerMedKontakt { get; set; }

        public string PipelineVersjon { get; set; }
        public IEnumerable<string> Enhetsinfo { get; set; }

        public ICollection<SmittekontaktDetaljerAm> Detaljer { get; set; }

        public bool HarKontaktDiagram { get; set; }
        public bool HarGpsHistogram { get; set; }

        public class Filter
        {
            public int? IndekspasientId { get; set; }
            public int? Sideindeks { get; set; }
            public int? Sideantall { get; set; }
        }
    }

    public class SmittekontaktDetaljerAm
    {
        public int SmittekontaktDetaljerId { get; set; }
        public DateTime Dato { get; set; }
        public Dictionary<string, double> Interessepunkter { get; set; }

        public double GpsAkkumulertVarighet { get; set; }
        public double GpsAkkumulertRisiko { get; set; }
        public double? GpsMedianavstand { get; set; }

        public double BluetoothAkkumulertVarighet { get; set; }
        public double BluetoothAkkumulertRisiko { get; set; }
        public double? BluetoothMedianavstand { get; set; }
        public double BluetoothVeldigNarVarighet { get; set; }
        public double BluetoothRelativtNarVarighet { get; set; }
        public double BluetoothNarVarighet { get; set; }

        public bool HarKart { get; set; }
    }

    public class SmittekontaktPersonopplysningerAm
    {
        public string Telefonnummer { get; set; }
    }
}
