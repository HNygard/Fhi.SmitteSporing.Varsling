using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Sms.Applikasjonsmodell.Modeller;
using Fhi.Sms.Konstanter.Enumer;
using Microsoft.Extensions.Options;
using MoreLinq.Extensions;
using Optional;

namespace Fhi.Smittesporing.Varsling.Eksternetjenester.Sms
{
    public class SmsTjenesteFacade : ISmsTjenesteFacade
    {
        private const int MottakerOpplastningBatchSize = 1000;

        private readonly SmsTjenesteKlient _smsKlient;
        private readonly SmsTjenesteKonfig _smsTjenesteKonfig;

        public SmsTjenesteFacade(SmsTjenesteKlient smsKlient, IOptions<SmsTjenesteKonfig> smsTjenesteKonfig)
        {
            _smsKlient = smsKlient;
            _smsTjenesteKonfig = smsTjenesteKonfig.Value;
        }

        public async Task<int> OpprettSmsJobb(int smsMalId, IEnumerable<SmsUtsending> smsUtsendinger)
        {
            var jobbId = await _smsKlient.OpprettSmsJobb(new SmsJobbAm.Opprett
            {
                SmsMalId = smsMalId,
                KildesystemId = _smsTjenesteKonfig.KildesystemId, // Brukes for gruppering av hendelser + fakturagrunnlag
                //KildesystemReferanse = ?, // Ikke nødvendig, men kan brukes for å koble SMS-jobb til indekspasient?
                //Sendetidspunkt = DateTime.Now.AddDays(1),
                //Mottakere = // Legges til i batcher gjennom egne kall etterpå -> Må håndtere "supersmittere"
            });

            foreach (var batch in smsUtsendinger.Batch(MottakerOpplastningBatchSize))
            {
                await _smsKlient.LeggTilSmsJobbMottakere(jobbId, batch.Select(x => new SmsJobbAm.SmsMottakerModell
                {
                    Mobil = x.Telefonnummer,
                    KildesystemReferanse = x.Referanse.ToString(),
                    Flettedata = x.Flettedata
                }));
            }

            return jobbId;
        }

        public async Task StartSmsJobb(int jobbId)
        {
            await _smsKlient.StartSmsJobb(jobbId);
        }

        public async Task<IEnumerable<SmsStatusoppdatering>> HentStatusoppdateringerEtterLopenummer(int lopenummer, int antall)
        {
            var smsHendelser = await _smsKlient.HentSmsHendelser(new SmsHendelseAm.ListeFilter
            {
                KildesystemId = _smsTjenesteKonfig.KildesystemId,
                Take = antall,
                MinimumId = lopenummer + 1
            }.Some());

            return smsHendelser.Select(MapTilStatusoppdatering);
        }

        public async Task<Option<SmsVarselMal>> HentSmsVarselMal(int malId)
        {
            var smsMal = await _smsKlient.HentSmsMal(malId);

            return smsMal.Map(x => new SmsVarselMal(x.Id, x.Avsender, x.Meldingsinnhold, x.KanEndres));
        }

        public async Task<int> LagreSmsVarselMal(SmsVarselMal mal)
        {
            if (mal.KanEndres && mal.SmsMalId != default)
            {
                await _smsKlient.OppdaterSmsMal(mal.SmsMalId, new SmsMalAm.Oppdater
                {
                    Meldingsinnhold = mal.Meldingsinnhold,
                    Avsender = mal.Avsender,
                    Navn = "Smittevarslingsmal"
                });
                return mal.SmsMalId;
            }
            else
            {
                return await _smsKlient.OpprettSmsMal(new SmsMalAm.Opprett
                {
                    Meldingsinnhold = mal.Meldingsinnhold,
                    Avsender = mal.Avsender,
                    Navn = "Smittevarslingsmal",
                    TidligereVersjonId = mal.SmsMalId == default ? (int?)null : mal.SmsMalId
                });
            }
        }

        public Task SendTestmeldingForMal(int malId, SmsUtsending smsUtsending)
        {
            return _smsKlient.SendTestutsendingForMal(malId, new SmsMalAm.SendTestutsending
            {
                Mobil = smsUtsending.Telefonnummer,
                Flettedata = smsUtsending.Flettedata,
                KildesystemId = _smsTjenesteKonfig.KildesystemId,
                KildesystemReferanse = smsUtsending.Referanse.ToString(),
                Bruksgruppe = "Smittesporing og varsling"
            });
        }

        public async Task<SmsTilgang> SjekkTilgang()
        {
            var tilgangsjekkAm = await _smsKlient.HentTilgangsjekk();
            
            return new SmsTilgang
            {
                Brukernavn = tilgangsjekkAm.Brukernavn,
                MalAdministrering = tilgangsjekkAm.MalAdministrering,
                SmsSending = tilgangsjekkAm.SmsSending
            };
        }

        public async Task<IEnumerable<SmsStatusoppdatering>> HentStatusoppdateringerForSms(Guid referanse)
        {
            var smsHendelser = await _smsKlient.HentSmsHendelser(new SmsHendelseAm.ListeFilter
            {
                KildesystemId = _smsTjenesteKonfig.KildesystemId,
                KildesystemReferanse = referanse.ToString()
            }.Some());

            return smsHendelser.Select(MapTilStatusoppdatering);
        }

        private SmsStatusoppdatering MapTilStatusoppdatering(SmsHendelseAm hendelse)
        {
            SmsStatus gjeldendeStatus;
            switch (hendelse.Utsending.Status)
            {
                case SmsUtsendingstatus.Venter:
                case SmsUtsendingstatus.UnderSending:
                    gjeldendeStatus = SmsStatus.Klargjort;
                    break;
                case SmsUtsendingstatus.Sendt:
                    gjeldendeStatus = SmsStatus.Sendt;
                    break;
                case SmsUtsendingstatus.Levert:
                    gjeldendeStatus = SmsStatus.Levert;
                    break;
                case SmsUtsendingstatus.Feilet:
                    gjeldendeStatus = SmsStatus.Feilet;
                    break;
                case SmsUtsendingstatus.DelvisLevert:
                    gjeldendeStatus = SmsStatus.DelvisLevert;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new SmsStatusoppdatering
            {
                Loepenummer = hendelse.Id,
                SmsUtsendingReferanse = OptionalParseGuid(hendelse.KildesystemReferanse),
                GjeldeneStatus = gjeldendeStatus,
                Tidspunkt = hendelse.Tidspunkt,
                Beskrivelse = hendelse.Beskrivelse,
                AntallSegmenter = hendelse.Utsending.SegmentAntall.ToOption()
            };
        }

        private Option<Guid> OptionalParseGuid(string value)
        {
            if (Guid.TryParse(value, out var guidValue))
            {
                return guidValue.Some();
            }
            return Option.None<Guid>();
        }
    }
}