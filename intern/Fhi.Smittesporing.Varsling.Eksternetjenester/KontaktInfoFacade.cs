using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Eksternetjenester
{
    public class KontaktInfoFacade : IKontaktInfoFacade
    {
        private readonly HttpClient _httpclient;
        private readonly string _key;

        public KontaktInfoFacade(HttpClient httpClient, IOptions<Konfig> konfig)
        {
            _key = konfig.Value.ApiKey;
            _httpclient = httpClient;
        }

        public async Task<KontaktinformasjonReponse> HentPersoner(IEnumerable<string> fnrListe)
        {
            var json = JsonConvert.SerializeObject(fnrListe);
            var content = new StringContent(json, Encoding.Default, "application/json");
            var httpResponseMessage = await _httpclient.PostAsync("hentPersoner?key=" + _key, content);

            httpResponseMessage.EnsureSuccessStatusCode();

            return await httpResponseMessage.Content.ReadAsAsync<KontaktinformasjonReponse>();
        }

        public class Konfig
        {
            public string ApiKey { get; set; }
        }
    }
}
