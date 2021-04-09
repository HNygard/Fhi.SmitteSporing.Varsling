using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public class SmittekontaktDetaljer : IOpprettetAv, ISistOppdatertAv
    {
        public int SmittekontaktDetaljerId { get; set; }
        public int SmittekontaktId { get; set; }
        public int OppsummertPlotHtmlId { get; set; }
        public DateTime Dato { get; set; }

        public string InteressepunkterJson { get; set; }

        public double GpsAkkumulertVarighet { get; set; }
        public double GpsAkkumulertRisiko { get; set; }
        public double? GpsMedianavstand { get; set; }
        [Obsolete("Gammel egenskap fra Simula-rapport, beholdes i en overgangsperiode")]
        public string GpsInteressepunkterJson { get; set; }

        public double BluetoothAkkumulertVarighet { get; set; }
        public double BluetoothAkkumulertRisiko { get; set; }
        public double? BluetoothMedianavstand { get; set; }
        [Obsolete("Gammel egenskap fra Simula-rapport, beholdes i en overgangsperiode")]
        public string BluetoothInteressepunkterJson { get; set; }
        public double BluetoothVeldigNarVarighet { get; set; }
        public double BluetoothRelativtNarVarighet { get; set; }
        public double BluetoothNarVarighet { get; set; }

        // EF Navigasjonsegenskaper
        [ForeignKey(nameof(OppsummertPlotHtmlId))]
        public SmittekontaktDetaljerHtmlKart OppsummertPlotDetaljerHtml { get; set; }
        [ForeignKey(nameof(SmittekontaktId))]
        public Smittekontakt Smittekontakt { get; set; }

        // Andre hjelpeegenskaper
        [NotMapped]
        public Dictionary<string, double> Interessepunkter
        {
            get => JsonConvert.DeserializeObject<Dictionary<string, double>>(InteressepunkterJson ?? "null");
            set => InteressepunkterJson = JsonConvert.SerializeObject(value);
        }
        [NotMapped]
        [Obsolete("Gammel egenskap fra Simula-rapport, beholdes i en overgangsperiode")]
        public Dictionary<string, double> GpsInteressepunkter
        {
            get => JsonConvert.DeserializeObject<Dictionary<string, double>>(GpsInteressepunkterJson ?? "null");
            set => GpsInteressepunkterJson = JsonConvert.SerializeObject(value);
        }
        [NotMapped]
        [Obsolete("Gammel egenskap fra Simula-rapport, beholdes i en overgangsperiode")]
        public Dictionary<string, double> BluetoothInteressepunkter
        {
            get => JsonConvert.DeserializeObject<Dictionary<string, double>>(BluetoothInteressepunkterJson ?? "null");
            set => BluetoothInteressepunkterJson = JsonConvert.SerializeObject(value);
        }

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }
    }



    public class SmittekontaktDetaljerMedKartinfo
    {
        public SmittekontaktDetaljer SmittekontaktDetaljer { get; set; }
        public bool HarKart { get; set; }
    }
}