using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Newtonsoft.Json;
using Optional;

namespace Fhi.Smittesporing.Simula.InternKlient
{
    public class SimulaInternKlient : ISimulaInternKlient
    {
        private readonly HttpClient _httpclient;

        public SimulaInternKlient(HttpClient httpclient)
        {
            _httpclient = httpclient;
        }

        public async Task<Option<Guid>> OpprettKontaktrapport(SimulaKontaktrapport.OpprettCommand command)
        {
            var requestJson = JsonConvert.SerializeObject(command);
            var response = await _httpclient.PostAsync("Kontaktrapporter", new StringContent(requestJson, Encoding.UTF8, "application/json"));

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Option.None<Guid>();
            }
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<SimulaKontaktrapport.OpprettResult>();

            return result.Id.Some();
        }

        public async Task<Option<SimulaKontaktrapport>> HentKontaktrapport(Guid id)
        {
            var response = await _httpclient.GetAsync($"Kontaktrapporter/{id}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Option.None<SimulaKontaktrapport>();
            }

            response.EnsureSuccessStatusCode();

            var simulaReponse = await response.Content.ReadAsAsync<SimulaKontaktrapport>();

            return simulaReponse.Some();
        }

        public async Task<ServerVersjonAm> HentVersjon()
        {
            var response = await _httpclient.GetAsync("ServerInfo/Versjon");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<ServerVersjonAm>();
        }

        public async Task<ServerVersjonAm> HentProxyVersjon()
        {
            var response = await _httpclient.GetAsync("ServerInfo/ProxyVersjon");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<ServerVersjonAm>();
        }

        public async Task<List<string>> HentSlettinger(IEnumerable<string> telefonnummer)
        {
            var requestJson = JsonConvert.SerializeObject(telefonnummer);
            var response = await _httpclient.PostAsync("Slettinger", new StringContent(requestJson, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<List<string>>();
        }

        public async Task<PagedListAm<SimulaDataBruk>> HentLoggOverBruk(SimulaDataBruk.HentCommand command)
        {
            var requestJson = JsonConvert.SerializeObject(command);
            var response = await _httpclient.PostAsync("Innsyn/LoggOverBruk", new StringContent(requestJson, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<PagedListAm<SimulaDataBruk>>();
        }

        public async Task<PagedListAm<SimulaGpsData>> HentGpsData(SimulaGpsData.HentCommand command)
        {
            var requestJson = JsonConvert.SerializeObject(command);
            var response = await _httpclient.PostAsync("Innsyn/GpsData", new StringContent(requestJson, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<PagedListAm<SimulaGpsData>>();
        }
    }
}
