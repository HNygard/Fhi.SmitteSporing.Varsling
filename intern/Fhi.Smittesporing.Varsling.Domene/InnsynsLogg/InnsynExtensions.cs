using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Intern.Applikasjonsmodell;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fhi.Smittesporing.Varsling.Domene.InnsynsLogg
{
    public static class InnsynExtensions
    {
        public const string HELSENORGEBRUKER = "KJENTBRUKERNAVNFORHELSENORGEBRUKER"; //TODO: Fyll ut rett bruker
        public const string OPPSLAGVIAHELSENORGENO = "Oppslag via Helsenorge.no";
        private const string DELIMITER = " - ";

        public static InnsynHelsenorgeAm MapToHelsenorgeAm(
            string telefonnummer,
            string fodselsnummer,
            IEnumerable<InnsynIndekspasientAm> indekspasient,
            IEnumerable<InnsynSmittekontaktAm> smittekontakter,
            IEnumerable<InnsynSmsVarselAm> smsvarsel
            )
        {
            return new InnsynHelsenorgeAm //TODO: Ta ut mapping i automapper (mest mulig)
            {
                Telefonnummer = telefonnummer,
                Fodselsnummer = fodselsnummer,
                Prøvedatoer = indekspasient.Where(pasient => pasient.Provedato.HasValue).Select(pasient => pasient.Provedato.Value),
                Smittekontakter = smittekontakter.Select(smk => new InnsynHelsenorgeSmittekontaktAm
                {
                    Dato = smk.Created,
                    Risikokategori = smk.Risikokategori,
                    Varslet = smk.VarsletTidspunkt,
                    Verifiseringskode = smk.Verifiseringskode
                }),
                SmsVarsel = smsvarsel.Select(sms => new InnsynHelsenorgeSmsvarselAm
                {
                    Tidspunkt = sms.SisteEksterneHendelsestidspunkt ?? sms.Created,
                    Status = sms.Status,
                    Kode = sms.Verifiseringskode
                })
            };
        }

        public static LoggSøk.Command AsLoggCommand(this InnsynHelsenorgeRequestAm request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var fnr = request.Fodselsnummer;

            if (!string.IsNullOrWhiteSpace(request.AktorFodselsnummer))
            {
                fnr = request.AktorFodselsnummer;
            }

            var loggedName = $"({fnr}){DELIMITER}{request.AktorName}";

            return new LoggSøk.Command
            {
                Fodselsnummer = request.Fodselsnummer,
                Formal = OPPSLAGVIAHELSENORGENO,
                Navn = loggedName
            };
        }

        public static IEnumerable<InnsynLoggHelsenorgeAm> ToHelsenorgeFormat(this IEnumerable<InnsynLoggAm> input)
        {
            if (input == null)
            {
                return Enumerable.Empty<InnsynLoggHelsenorgeAm>();
            }

            return input.Select(logg => new InnsynLoggHelsenorgeAm
            {
                Dato = logg.Created,
                Formal = logg.Hvorfor,
                Navn = logg.Hvem,
                Organisasjon = string.Empty
            });
        }

        public static IEnumerable<InnsynLoggHelsenorgeAm> EkskluderInnbygger(this IEnumerable<InnsynLoggHelsenorgeAm> input, InnsynFilterAm filter)
        {
            if (input == null)
            {
                return null;
            }

            // Ved innsyn via helsenorge logges navn som {fnr - <navn>}
            // Så hvis _dette_ søket er for fnr1 og logg.Name starter med fnr1
            // da er det personen selv som har gjort søket og det kan fjernes
            var filteredInput = input.Where(x => x.Navn.StartsWith($"({filter.Fodselsnummer}") == false);

            return filteredInput;
        }

        public static IEnumerable<InnsynLoggHelsenorgeAm> Reduce(this IEnumerable<InnsynLoggHelsenorgeAm> input)
        {
            if (input == null)
            {
                return null;
            }

            if (input.Count() == 1)
            {
                return input;
            }

            var comparer = new InnsynLoggHelsenorgeAmEqualityComparer();

            InnsynLoggHelsenorgeAm previous = null;

            var output = new List<InnsynLoggHelsenorgeAm>();

            foreach (var current in input)
            {
                if (previous == null)
                {
                    previous = current;
                    output.Add(current);
                }
                else
                {
                    if (!comparer.Equals(current, previous))
                    {
                        output.Add(current);
                    }
                }

                previous = current;
            }

            return output;
        }

        public static IEnumerable<InnsynLoggHelsenorgeAm> Pseudonymiser(this IEnumerable<InnsynLoggHelsenorgeAm> input)
        {
            if (input == null)
            {
                return null;
            }

            return input.PseudonymiserInternal();
        }

        private static IEnumerable<InnsynLoggHelsenorgeAm> PseudonymiserInternal(this IEnumerable<InnsynLoggHelsenorgeAm> input)
        {
            if (input == null)
            {
                yield break;
            }

            var alleHvem = input.Select(innsyn => innsyn.Navn).Distinct();

            var fhiBrukere = alleHvem.Where(ErFhiAnsatt).ToArray();

            foreach (var innsyn in input)
            {
                if (ErFhiAnsatt(innsyn.Navn))
                {
                    innsyn.Navn = $"Smittejeger {Array.IndexOf(fhiBrukere, innsyn.Navn) + 1}";
                    innsyn.Organisasjon = "FHI";
                }
                else if (ErHelsenorgeSystemBruker(innsyn.Navn))
                {
                    innsyn.Navn = "Systembruker";
                    innsyn.Organisasjon = "FHI";
                    innsyn.Formal = OPPSLAGVIAHELSENORGENO;
                }
                else if (ErSystemBruker(innsyn.Navn))
                {
                    innsyn.Navn = $"Smittejeger {Array.IndexOf(fhiBrukere, innsyn.Navn) + 1}";
                    innsyn.Organisasjon = "FHI";
                }
                else if (ErMuligPersonligBruker(innsyn.Navn))
                {
                    var index = innsyn.Navn.IndexOf(DELIMITER);
                    innsyn.Navn = innsyn.Navn.Substring(index + DELIMITER.Length);
                }

                yield return innsyn;
            }
        }

        private static bool ErHelsenorgeSystemBruker(string brukernavn)
        {
            if (string.IsNullOrWhiteSpace(brukernavn))
            {
                return false;
            }

            if (HELSENORGEBRUKER.Equals(brukernavn))
            {
                return true;
            }

            return false;
        }

        private static bool ErSystemBruker(string brukernavn)
        {
            if (string.IsNullOrWhiteSpace(brukernavn))
            {
                return false;
            }

            if (ErHelsenorgeSystemBruker(brukernavn))
            {
                return false;
            }

            return brukernavn.StartsWith(@"fhi\", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ErMuligPersonligBruker(string brukernavn)
        {
            if (string.IsNullOrWhiteSpace(brukernavn))
            {
                return false;
            }

            return brukernavn.StartsWith("(") && brukernavn.Contains(DELIMITER);
        }
        private static bool ErFhiAnsatt(string brukernavn)
        {
            if (string.IsNullOrWhiteSpace(brukernavn))
            {
                return false;
            }

            if (ErHelsenorgeSystemBruker(brukernavn))
            {
                return false;
            }

            return brukernavn.StartsWith("fhi", StringComparison.OrdinalIgnoreCase);
        }
    }
}
