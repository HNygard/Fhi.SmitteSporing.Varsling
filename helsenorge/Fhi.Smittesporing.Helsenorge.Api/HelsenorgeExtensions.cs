using Fhi.Smittesporing.Helsenorge.Api.Models;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fhi.Smittesporing.Helsenorge.Api
{
    public static class UriExtensions
    {
        public static string AddQueryString(this string url, string name, string value)
        {
            return QueryHelpers.AddQueryString(url, name, value);
        }
    }

    public static class HelsenorgeExtensions
    {
        private const string DATEFORMAT = "dd.MM.yyyy HH:mm";
        private const string SHORTDATEFORMAT = "dd.MM.yyyy";
        public static Innsyn AsInnsyn(this IEnumerable<InnsynLoggHn> input)
        {
            //hvis loggen ikke har noe innhold (feil, ingenting å vise, eller ikke treff) vis en melding for dette
            //ellers grupper innsyn på år og vis som tabeller
            if (input == null) // Skal bare skje ved feil i bakenforliggende system
            {
                return CreateInnsynloggTomMedFeilmelding();
            }
            else if (!input.Any()) // Tom logg
            {
                return CreateInnsynLoggTom();
            }
            else if (input.Count() >= 5000) // Gi melding om at det _kan_ være flere treff i basen og ta kontakt for å få komplett innsynslogg etc
            {
                return CreateInnsynLoggÅr(input, detFinnesFlereRader: true);
            }
            else // Vanlig resultat
            {
                return CreateInnsynLoggÅr(input);
            }
        }

        public static Innsyn AsInnsyn(this InnsynHn person)
        {
            return new Innsyn
            {
                Section = new[]
                {
                    CreatePrøvedatoerSeksjon(person.Prøvedatoer),
                    CreateSmittekontakterSeksjon(person.Smittekontakter),
                    CreateSmsVarselSeksjon(person.SmsVarsel),
                }
            };
        }

        private static Section CreateSmsVarselSeksjon(IEnumerable<SmsVarselHn> smsvarsel)
        {
            return new Section
            {
                title = "Smsvarsel",
                Items = new object[]
                {
                    SettInnParagraf("Sms-varsel som har blitt sendt til innbygger."),
                    new Table
                    {
                        Header = new[] {"Tidspunkt", "Status", "Kode"},
                        Rader = CreateSmsRader(smsvarsel)
                    }
                }
            };
        }

        private static Section CreateSmittekontakterSeksjon(IEnumerable<SmitteKontaktHn> smittekontakter)
        {
            return new Section
            {
                title = "Smittekontakter",
                Items = new object[]
                {
                    SettInnParagraf("En smittekontakt er når innbygger har vært i kontakt med en annen innbygger smittet med Covid-19."),
                    new Table
                    {
                        Header = new[] {"Dato", "Risikokategori", "Varsel sendt"},
                        Rader = CreateSmittekontaktRader(smittekontakter)
                    }
                }
            };
        }

        private static Section CreatePrøvedatoerSeksjon(IEnumerable<DateTime> prøvedatoer)
        {
            return new Section
            {
                title = "Prøvedatoer",
                Items = new object[]
                {
                    //SettInnOverskrift(""),
                    SettInnParagraf("Prøvedatoer for positive prøver."),
                    new UnorderedList
                    {
                        ListItem = prøvedatoer.Select(dato => new Text{ Text1 = new [] { dato.ToString(SHORTDATEFORMAT) } } ).ToArray()
                    }
                    //new Table
                    //{
                    //    Header = new [] { "Prøvedato" },
                    //    Rader = prøvedatoer.Select(dato => new RadInnhold { Cell = new [] { dato.ToString(SHORTDATEFORMAT) } }).ToArray()
                    //}
                }
            };
        }

        private static Innsyn CreateInnsynloggTomMedFeilmelding()
        =>
            new Innsyn()
            {
                Section = new Section[] {
                CreateInnsynLoggInfoSection(),
                new Section()
                {
                    title = "Feil",
                    Items = new object[] {
                        new Text
                        {
                            paragraph = true,
                            Text1 = new[] { "Det oppstod en feil under kommunikasjon med bakenforliggende system, vennligst prøv igjen senere." }
                        }
                    }
                }
                }
            };

        private static Innsyn CreateInnsynLoggTom()
        =>
            new Innsyn()
            {
                Section = new Section[] {
                CreateInnsynLoggInfoSection(),
                new Section()
                {
                    Items = new object[] {
                        new Text
                        {
                            paragraph = true,
                            Text1 = new[] { "Det finnes i øyeblikket ingen innsyn å vise." }
                        }
                    }
                }
                }
            };

        private static Innsyn CreateInnsynLoggÅr(IEnumerable<InnsynLoggHn> logg, bool detFinnesFlereRader = false)
        {
            List<Section> sections = new List<Section>();
            var alleÅr = logg.Select(l => l.Dato.Year).Distinct().OrderByDescending(l => l).ToArray();

            sections.Add(CreateInnsynLoggInfoSection());

            if (detFinnesFlereRader)
            {
                sections.Add(CreateInnsynLoggFlereRaderSection());
            }

            foreach (var år in alleÅr)
            {
                sections.Add(CreateInnsynLoggSection(år, logg.Where(l => l.Dato.Year == år), år == alleÅr.Max()));
            }

            return new Innsyn
            {
                Section = sections.ToArray()
            };
        }

        private static Section CreateInnsynLoggFlereRaderSection()
           =>
                new Section()
                {
                    title = "Informasjon",
                    collapsed = true,
                    Items = new object[]
                    {
                        SettInnOverskrift("Logg over bruk:"),
                        SettInnParagraf("Det returneres bare 5000 rader fra din innsynslogg via denne tjenesten."),
                        //SettInnTomLinje(),
                        SettInnParagraf("Hvis du ønsker et komplett bilde av din innsynslogg må du ta kontakt med FHI direkte.")
                    }
                };

        // TODO: Samkjør tekstene
        private static Section CreateInnsynLoggInfoSection()
            =>
                new Section()
                {
                    title = "Informasjon",
                    collapsed = true,
                    Items = new object[]
                    {
                        SettInnOverskrift("Logg over bruk:"),
                        SettInnParagraf("Her ser du en oversikt over hvem som har sett på eller fått utlevert data som er registrert om deg."),
                        //SettInnTomLinje(),
                        SettInnOverskrift("Det finnes flere måter å se på opplysningene:"),
                        //SettInnTomLinje(),
                        SettInnOverskrift("Innsyn via helsenorge.no"),
                        SettInnParagraf("Innsyn via helsenorge.no vil si at helseopplysninger fra registeret er hentet fram når du eller andre med tilgang til dine helseopplysninger på helsenorge.no har logget inn og sett dem."),
                        //SettInnTomLinje(),
                        SettInnOverskrift("Kvalitetssikring av varsler"),
                        SettInnParagraf("Kvalitetssikring på varsler betyr at en saksbehandler har sett spesifikt på dine opplysninger, for å kvalitetssikre informasjon. Dette kan for eksempel være på grunn av mangler eller feil i innrapporterte opplysninger."),
                        //SettInnTomLinje(),
                        SettInnOverskrift("Dine rettigheter"),
                        SettInnParagraf("Ansatte ved Folkehelseinstituttet har taushetsplikt ifølge Helseregisterloven § 17, jf. Helsepersonelloven § 21. Les mer om din rett til informasjon og innsyn på fhi.no, se https://www.fhi.no/div/personvern/til-allmennheten/rett-til-informasjon-om-innsyn-i-og/")
                    }
                };

        private static Text SettInnOverskrift(string overskrift)
            => new Text()
            {
                Text1 = new[] { overskrift },
                emphasized = true,
                @break = true
            };

        private static Text SettInnParagraf(string brødtekst)
            => new Text()
            {
                Text1 = new[] { brødtekst },
                @break = true,
                paragraph = true
            };

        private static Text SettInnTomLinje()
            => new Text()
            {
                Text1 = new[] { "" },
                @break = true
            };

        private static Section CreateInnsynLoggSection(int år, IEnumerable<InnsynLoggHn> logg, bool sisteÅr)
        =>
                new Section
                {
                    title = år.ToString(),
                    collapsed = !sisteÅr,
                    Items = new object[]
                    {
                        new Table
                        {
                            Header = new[] {"Når", "Hvem", "Virksomhet", "Bakgrunn"}, //TODO: Samkjør overskriftene
                            Rader = CreateInnsynLoggRader(logg)
                        }
                    }
                };

        private static RadInnhold[] CreateInnsynLoggRader(IEnumerable<InnsynLoggHn> logg)
        {
            return logg.Select(oppføring => new RadInnhold
            {
                Cell = new[]
                {
                    oppføring.Dato.ToString(DATEFORMAT),
                    oppføring.Navn,
                    oppføring.Organisasjon,
                    oppføring.Formal
                }
            }).ToArray();
        }

        private static RadInnhold[] CreateSmittekontaktRader(IEnumerable<SmitteKontaktHn> smittekontakter)
        {
            return smittekontakter.Select(smittekontakt => new RadInnhold
            {
                Cell = new[]
                {
                    smittekontakt.Dato.ToString(DATEFORMAT),
                    smittekontakt.Risikokategori,
                    smittekontakt.Varslet?.ToString(DATEFORMAT)
                }
            }).ToArray();
        }

        private static RadInnhold[] CreateSmsRader(IEnumerable<SmsVarselHn> smsvarsel)
        {
            return smsvarsel.Select(sms => new RadInnhold
            {
                Cell = new[]
                {
                    sms.Tidspunkt.ToString(DATEFORMAT),
                    sms.Status,
                    sms.Kode
                }
            }).ToArray();
        }
    }
}
