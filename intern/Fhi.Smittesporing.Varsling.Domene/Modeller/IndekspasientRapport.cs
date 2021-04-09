using System;
using System.Collections.Generic;
using System.Linq;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Rapport;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public class IndekspasientRapport
    {
        public IndekspasientRapport(Indekspasient.Filter filter, IEnumerable<IndekspasientData> tilfelleData)
        {
            Generertdato = DateTime.Now;

            var labels = new List<string>();
            var antallIndekspasienterMedKontaktVerdier = new List<int>();
            var antallIndekspasienterUtenKontaktVerdier = new List<int>();
            var antallKontakterVerdier = new List<int>();
            DateTime? forsteOpprettetTidspunkt;
            DateTime? sisteOpprettetTidspunkt = null;
            var antallTilfellerSisteDag = 0;

            using (var enumerator = tilfelleData.GetEnumerator())
            {
                enumerator.MoveNext();
                var gjeldendePasient = enumerator.Current;
                forsteOpprettetTidspunkt = gjeldendePasient?.OpprettetTidspunkt;

                var fraOgMedDato = filter.FraOgMed.ValueOr(() => (forsteOpprettetTidspunkt ?? DateTime.Now).Date);
                var tilOgMedDato = filter.TilOgMed.ValueOr(() => DateTime.Now);

                var gjeldendeDato = fraOgMedDato;
                while (gjeldendeDato <= tilOgMedDato)
                {
                    var gjeldendeDatoCutoff = gjeldendeDato.AddDays(1);
                    var antallIndekspasienterMedKontakt = 0;
                    var antallIndekspasienterUtenKontakt = 0;
                    var antallKontakter = 0;
                    while (gjeldendePasient != null && gjeldendePasient.OpprettetTidspunkt < gjeldendeDatoCutoff)
                    {
                        antallKontakter += gjeldendePasient.AntallSmittekontakter;
                        if (gjeldendePasient.Status == IndekspasientStatus.SmitteKontakt)
                        {
                            antallIndekspasienterMedKontakt++;
                        }
                        if (gjeldendePasient.Status == IndekspasientStatus.IkkeSmitteKontakt)
                        {
                            antallIndekspasienterUtenKontakt++;
                        }
                        sisteOpprettetTidspunkt = gjeldendePasient.OpprettetTidspunkt;
                        enumerator.MoveNext();
                        gjeldendePasient = enumerator.Current;
                    }

                    labels.Add(gjeldendeDato.ToString("ddd"));
                    antallIndekspasienterMedKontaktVerdier.Add(antallIndekspasienterMedKontakt);
                    antallIndekspasienterUtenKontaktVerdier.Add(antallIndekspasienterUtenKontakt);
                    antallKontakterVerdier.Add(antallKontakter);
                    antallTilfellerSisteDag = antallIndekspasienterMedKontakt + antallIndekspasienterUtenKontakt;

                    gjeldendeDato = gjeldendeDatoCutoff;
                }

                Periode = new Periode(fraOgMedDato, tilOgMedDato);
            }

            SisteOpprettet = sisteOpprettetTidspunkt;
            AntallSisteDag = antallTilfellerSisteDag;
            ChartData = new ChartData(
                labels,
                new SerieAm("Nye indekspasienter med kontakt", antallIndekspasienterMedKontaktVerdier),
                new SerieAm("Nye indekspasienter uten kontakt", antallIndekspasienterUtenKontaktVerdier),
                new SerieAm("Nye smittekontakter", antallKontakterVerdier)
            );
        }

        public DateTime Generertdato { get; }
        public Periode Periode { get; }
        public DateTime? SisteOpprettet { get; }
        public int AntallSisteDag { get; }
        public ChartData ChartData { get; }

        public class IndekspasientData
        {
            public DateTime OpprettetTidspunkt { get; set; }
            public IndekspasientStatus Status { get; set; }
            public int IndekspasientId { get; set; }
            public int AntallSmittekontakter { get; set; }
        }
    }
    public class Periode
    {
        public Periode(DateTime fraOgMedTidspunkt, DateTime tilOgMedTidspunkt)
        {
            FraOgMedTidspunkt = fraOgMedTidspunkt;
            TilOgMedTidspunkt = tilOgMedTidspunkt;
        }

        public DateTime FraOgMedTidspunkt { get; }
        public DateTime TilOgMedTidspunkt { get; }

        /// <summary>
        /// Gets the yyyy-12-31 23:59:59 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteEndOfYear(int year)
        {
            return new DateTime(year, 12, 31).AddDays(1).AddTicks(-1);
        }

        public override string ToString()
        {
            return FraOgMedTidspunkt.ToShortDateString() + " - " + TilOgMedTidspunkt.ToShortDateString();
        }
    }

    public class ChartData
    {
        public ChartData(IEnumerable<string> labels, params SerieAm[] serier)
        {
            Labels = labels.ToList();
            Series = serier.ToList();
        }
        public List<SerieAm> Series { get; }
        public List<string> Labels { get; }
    }
}