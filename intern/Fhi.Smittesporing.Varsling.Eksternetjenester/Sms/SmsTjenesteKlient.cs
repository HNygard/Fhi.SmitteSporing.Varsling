using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Fhi.Sms.Applikasjonsmodell.Modeller;
using Fhi.Sms.Konstanter;
using Optional;

namespace Fhi.Smittesporing.Varsling.Eksternetjenester.Sms
{
    public class SmsTjenesteKlient
    {
        private readonly HttpClient _httpClient;

        public SmsTjenesteKlient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> OpprettSmsJobb(SmsJobbAm.Opprett command)
        {
            var idAm = await _httpClient.PostJson<IdAm>("SmsJobber", command);
            return idAm.Id;
        }

        public async Task LeggTilSmsJobbMottakere(int jobbId, IEnumerable<SmsJobbAm.SmsMottakerModell> command)
        {
            var response = await _httpClient.PostJson($"SmsJobber/{jobbId}/Mottakerliste", command);
            response.EnsureSuccessStatusCode();
        }

        public async Task StartSmsJobb(int jobbId)
        {
            var response = await _httpClient.PostJson($"SmsJobber/{jobbId}/Start", null);
            response.EnsureSuccessStatusCode();
        }

        public Task<List<SmsHendelseAm>> HentSmsHendelser(Option<SmsHendelseAm.ListeFilter> filter = default)
        {
            return _httpClient.GetJson<List<SmsHendelseAm>>("SmsHendelser".LeggTilOptionalQuery(filter));
        }

        public Task<Option<SmsMalAm>> HentSmsMal(int malId)
        {
            return _httpClient.OptionalGetJson<SmsMalAm>($"SmsMaler/{malId}");
        }

        public async Task OppdaterSmsMal(int malId, SmsMalAm.Oppdater oppdater)
        {
            var response = await _httpClient.PutAsync($"SmsMaler/{malId}", oppdater.SomJson());
            response.EnsureSuccessStatusCode();
        }

        public async Task<int> OpprettSmsMal(SmsMalAm.Opprett opprett)
        {
            var idAm = await _httpClient.PostJson<IdAm>("SmsMaler", opprett);
            return idAm.Id;
        }

        public async Task SendTestutsendingForMal(int malId, SmsMalAm.SendTestutsending sendTestutsending)
        {
            var response = await _httpClient.PostJson($"SmsMaler/{malId}/SendTestutsending", sendTestutsending);
            response.EnsureSuccessStatusCode();
        }

        public Task<TilgangsjekkAm> HentTilgangsjekk()
        {
            return _httpClient.GetJson<TilgangsjekkAm>(ApiRoutes.Tilgangssjekk.RoutePrefix);
        }
    }
}