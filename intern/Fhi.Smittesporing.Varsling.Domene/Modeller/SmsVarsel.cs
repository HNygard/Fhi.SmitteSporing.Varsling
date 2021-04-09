using System;
using System.Linq;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public enum SmsStatus
    {
        Opprettet = 0,
        Klargjort = 1,
        Sendt = 2,
        Levert = 3,
        Feilet = 4,
        DelvisLevert = 5,
        Ukjent = 6
    }

    public class SmsVarsel : IOpprettetAv, ISistOppdatertAv
    {
        public int SmsVarselId { get; set; }

        public SmsStatus Status { get; set; } = SmsStatus.Opprettet;

        public DateTime Created { get; set; }
        public string OpprettetAv { get; set; }
        public string SistOppdatertAv { get; set; }
        public DateTime? SistOppdatert { get; set; }

        // OBS! Denne DateTime'n er UTC og _ikke_ localtime som alle de andre
        public DateTime? SisteEksterneHendelsestidspunkt { get; set; }

        public Guid? Referanse { get; set; }
        public int SmittekontaktId { get; set; }
        public Smittekontakt Smittekontakt { get; set; }

        public SmsUtsending LagUtsending(ICryptoManagerFacade cryptoManager, SmsFletteinnstillinger innstillinger)
        {
            if (Referanse == null)
            {
                Referanse = Guid.NewGuid();
            }
            return new SmsUtsending
            {
                Telefonnummer = cryptoManager.DekrypterUtenBrukerinnsyn(Smittekontakt.Telefon.Telefonnummer),
                Referanse = Referanse.Value,
                Flettedata = SmsVarselMal.TilgjengeligeFlettefelter.ToDictionary(
                    f => f.Navn,
                    f => f.HentVerdi(Smittekontakt, innstillinger)
                )
            };
        }

    }
}