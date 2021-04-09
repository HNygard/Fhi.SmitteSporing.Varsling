using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Domene;
using Newtonsoft.Json;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public class Smittekontakt : IOpprettetAv, ISistOppdatertAv
    {
        // DB-kolonner
        public int SmittekontaktId { get; set; }
        public DateTime? VarsletTidspunkt { get; set; }
        public int IndekspasientId { get; set; }
        public int TelefonId { get; set; }

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }

        /// <summary>
        /// Kode som brukes i SMS-varsler for verifisering av varsel via app.
        /// </summary>
        public string Verifiseringskode { get; set; }

        public string Risikokategori { get; set; }
        public int AntallKontakter { get; set; }
        public int? AntallDagerMedKontakt { get; set; }
        public string InteressepunkterJson { get; set; }

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
        public string EnhetsinfoJson { get; set; }

        // EF Navigasjonsegenskaper
        [ForeignKey(nameof(IndekspasientId))]
        public Indekspasient Indekspasient { get; set; }
        [ForeignKey(nameof(TelefonId))]
        public Telefon Telefon { get; set; }
        public SmittekontaktDiagram SoyleDiagram { get; set; }
        public SmittekontaktGpsHistogram GpsHistogram { get; set; }
        public ICollection<SmsVarsel> SmsVarsler { get; set; }
        public ICollection<SmittekontaktDetaljer> Detaljer { get; set; }

        // Andre hjelpeegenskaper
        [NotMapped]
        public Dictionary<string, double> Interessepunkter
        {
            get => JsonConvert.DeserializeObject<Dictionary<string, double>>(InteressepunkterJson ?? "null");
            set => InteressepunkterJson = JsonConvert.SerializeObject(value);
        }
        [NotMapped]
        public List<string> Enhetsinfo
        {
            get => JsonConvert.DeserializeObject<List<string>>(EnhetsinfoJson ?? "null");
            set => EnhetsinfoJson = JsonConvert.SerializeObject(value);
        }

        public class Filter : IPagedQuery
        {
            public Option<int> IndekspasientId { get; set; }
            public Option<int> Sideindeks { get; set; }
            public Option<int> Sideantall { get; set; }
        }
    }

    public class SmittekontaktOgSisteKontaktdato
    {
        public Smittekontakt Smittekontakt { get; set; }
        public DateTime? SisteKontaktdato { get; set; }
    }

    public class SmittekontaktMedDetaljer
    {
        public Smittekontakt Smittekontakt { get; set; }
        public bool HarKontaktDiagram { get; set; }
        public bool HarGpsHistogram { get; set; }
        public IEnumerable<SmittekontaktDetaljerMedKartinfo> Detaljer { get; set; }
    }
}
