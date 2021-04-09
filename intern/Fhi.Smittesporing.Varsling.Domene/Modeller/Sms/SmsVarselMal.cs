using System;
using System.Globalization;
using System.Linq;
using Optional;
using Optional.Collections;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Sms
{
    public class SmsVarselMal
    {
        private static readonly CultureInfo SmsCultureInfo = new CultureInfo("no");

        public SmsVarselMal(string avsender, string innhold)
        {
            Avsender = avsender;
            Meldingsinnhold = innhold;
        }

        public SmsVarselMal(int id, string avsender, string innhold, bool kanEndres) : this(avsender, innhold)
        {
            SmsMalId = id;
            KanEndres = kanEndres;
        }

        public int SmsMalId { get; set; }
        public string Meldingsinnhold { get; set; }
        public string Avsender { get; set; }
        /// <summary>
        /// Når en SMS-mal har utsendinger tillater ikke SMS-tjenesten endringer lenger
        /// -> Endringer må lagres som ny mal
        /// </summary>
        public bool KanEndres { get; set; }

        public static SmsFlettefelt[] TilgjengeligeFlettefelter =
        {
            new SmsFlettefelt(
                "kontaktDato", 
                "Dato for siste kontakt med smittet person", 
                DateTime.Now.ToString("d. MMMM yyyy", SmsCultureInfo),
                (k, i) => k.Detaljer
                    .Select(x => x.Dato)
                    .OrderByDescending(x => x)
                    .FirstOrNone()
                    .Map(x => x.ToString("d. MMMM yyyy", SmsCultureInfo))
                    .ValueOr("<ukjent dato>")),

            new SmsFlettefelt(
                "kommuneInfo",
                "Informasjon i SMS spesifikt for kommunen indekspasienten tilhører.",
                "Har du spørsmål kan du ta kontakt med kommunelegen på tlf +4798765432",
                (k, i) => k.Indekspasient.Kommune
                    .SomeNotNull()
                    .FlatMap(x => x.SmsFletteinfo.SomeNotNull())
                    .ValueOr(i.KommuneinfoFallback ?? string.Empty)),

            new SmsFlettefelt(
                "risikoTekst",
                "Innhold knyttet til risikokategori for kontakt.",
                new SmsFletteinnstillinger().MiddelsRisikokategori ?? "<risikotekst>", 
                (k, i) => k.Risikokategori
                    .SomeNotNull()
                    .FlatMap(r => i.TekstForRisiko(r))
                    .ValueOr(string.Empty)),

            new SmsFlettefelt(
                "verifiseringskode",
                "Verifiseringskode fra Smittestopp-app for SMS-varselet.",
                "7357",
                (k, i) => k.Verifiseringskode ?? "Ikke angitt")
        };
    }
}