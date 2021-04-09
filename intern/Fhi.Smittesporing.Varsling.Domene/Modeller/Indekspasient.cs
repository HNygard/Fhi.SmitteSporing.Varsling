using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Domene;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public class Indekspasient : IOpprettetAv, ISistOppdatertAv
    {
        // DB-Kolonner
        public int IndekspasientId { get; set; }
        /// <summary>
        /// Tidspunkt opprettet MSIS
        /// </summary>
        public DateTime Opprettettidspunkt { get;  set; }
        public DateTime? Provedato { get; set; }
        public int? KommuneId { get; set; }
        public int? TelefonId { get; set; }
        [AuditTrailOptions(SensitivVerdi = true)]
        public string Fodselsnummer { get; set; }

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }

        public IndekspasientStatus Status { get; set; } = IndekspasientStatus.Registrert;
        public Varslingsstatus Varslingsstatus { get; set; } = Varslingsstatus.TilGodkjenning;

        // EF navigasjonsegenskaper
        [ForeignKey(nameof(TelefonId))]
        public Telefon Telefon { get; set; }
        [ForeignKey(nameof(KommuneId))]
        public Kommune Kommune { get; set; }
        public ICollection<Smittekontakt> Smittekontakter { get; set; }

        public bool KanGodkjennesForVarsling => 
            Status == IndekspasientStatus.SmitteKontakt &&
            Varslingsstatus == Varslingsstatus.TilGodkjenning;

        public bool KanSettesFerdig => Varslingsstatus == Varslingsstatus.TilGodkjenning;
        public bool KanVisesTilBruker => StatuserKanVises.Contains(Status); // Kun relevant for test/dev

        public static IndekspasientStatus[] StatuserKanVises =
        {
            IndekspasientStatus.SmitteKontakt,
            IndekspasientStatus.IkkeSmitteKontakt,
            IndekspasientStatus.KontaktInfoMangler
        };

        public class Filter : IPagedQuery
        {
            public bool VisSkjulteStatuser { get; set; } = false;
            public Option<int> IndekspasientId { get; set; }
            public Option<bool> KreverGodkjenning { get; set; }
            public Option<bool> ErFerdig { get; set; }
            public Option<bool> ErRegistert { get; set; }
            public Option<bool> ManglerKontaktinfo { get; set; }
            public Option<bool> MedSmittekontakt { get; set; }
            public Option<string> KommuneNr { get; set; }
            public Option<string> Telefonnummer { get; set; }
            public Option<DateTime> FraOgMed { get; set; }
            public Option<DateTime> TilOgMed { get; set; }
            public Option<int> Sideindeks { get; set; }
            public Option<int> Sideantall { get; set; }
        }

        public void SlettData()
        {
            Status = IndekspasientStatus.Slettet;
            Opprettettidspunkt = default;
            Kommune = null;
            KommuneId = null;
            Telefon = null;
            TelefonId = null;
            Fodselsnummer = null;
            Provedato = null;
        }
    }

    public enum IndekspasientStatus
    {
        /// <summary>
        /// Registrert fra MSIS
        /// -> Mellomstatus før ManglerFodselsnummer/KontaktinfoMangler/IkkeSmittekontakt/Smittekontakt
        /// </summary>
        Registrert = 0,
        /// <summary>
        /// Kunne ikke finne kontaktinfo eller fikk tomt svar i sjekk mot simula
        /// Alle tilhørende data skal være slettet annen enn timestamps for sync og lookup
        /// </summary>
        Slettet = 1,
        /// <summary>
        /// TLF ikke funnet i difi-oppslag
        /// -> videre prosess gis opp
        /// </summary>
        KontaktInfoMangler = 2,
        /// <summary>
        /// Ingen smittekontakt registrert fra Smittestopp-app (Simula)
        /// -> videre prosess gis opp
        /// </summary>
        IkkeSmitteKontakt = 3,
        /// <summary>
        /// Smittekontakt registrert fra Smittestopp-app (Simula)
        /// -> Kan danne grunnlag for SMS-varsel
        /// </summary>
        SmitteKontakt = 4
    }

    public enum Varslingsstatus
    {
        /// <summary>
        /// Default: Ingen varsler sendes før godkjenning
        /// </summary>
        TilGodkjenning = 0,
        /// <summary>
        /// Smittetilfellet er godkjent for utsending av varsler og vil fanges opp av varslingsjobb
        /// </summary>
        Godkjent = 1,
        /// <summary>
        /// Status som brukes fra SMS-jobb er ferdig opprettet og frem til SMS-jobben er startet
        /// Brukes for unngå duplikate varsler hvis uventet feil oppstår under starting av SMS-jobb
        /// NB! Hvis vedvarende status -> sjekk opp mot SMS-tjeneste hva faktisk status er
        /// </summary>
        Klargjort = 2,
        /// <summary>
        /// Status for indekspasient når alle smittekontakter som skulle varsles har blitt overført til SMS-tjeneste
        /// og fått startet SMS-jobb OK
        /// </summary>
        Ferdig = 3
    }

    /// <summary>
    /// Wrapper for å hente Indekspasient med utvidet info i en samlet spørring
    /// </summary>
    public class IndekspasientOgAntall
    {
        public Indekspasient Indekspasient { get; set; }
        public int AntallSmittekontakter { get; set; }
    }

}
