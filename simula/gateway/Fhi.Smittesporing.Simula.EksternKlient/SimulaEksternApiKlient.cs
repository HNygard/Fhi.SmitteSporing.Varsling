using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Optional;

namespace Fhi.Smittesporing.Simula.EksternKlient
{
    public class SimulaEksternApiKlient : ISimulaEksternApiKlient
    {
        private readonly HttpClient _httpClient;

        public SimulaEksternApiKlient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Option<SimulaStartContactResult>> StartKontaktberegning(SimulaStartContactRequest request)
        {
            var requestJson = JsonConvert.SerializeObject(request);
            var result = await _httpClient.PostAsync("fhi/lookup", new StringContent(requestJson, Encoding.UTF8, "application/json"));
            if (result.IsSuccessStatusCode)
            {
                var responseJson = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<SimulaStartContactResult>(responseJson);
                return response.Some();
            }
            else
            {
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    // NotFound bruker samme body som Ok, men da med alle verdier = default
                    return Option.None<SimulaStartContactResult>();
                }
                var responseJson = await result.Content.ReadAsStringAsync();
                throw new Exception($"Kall mot Simula API ga feilkode {result.StatusCode}.{Environment.NewLine}{responseJson}");
            }
        }

        public async Task<Option<Option<SimulaContactReport, SimulaNotFinishedResult>>> HentKontaktresultat(Guid requestId)
        {
            var result = await _httpClient.GetAsync("fhi/lookup/" + requestId);
            switch (result.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    {
                        // Request-ID er gyldig, men rapporten er ikke klar enda
                        var responseJson = await result.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<SimulaNotFinishedResult>(responseJson);
                        return Option.None<SimulaContactReport, SimulaNotFinishedResult>(response).Some();
                    }

                case HttpStatusCode.OK:
                    {
                        // Request-ID er gyldig, og rapport er klar
                        var responseJson = await result.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<SimulaContactReport>(responseJson);
                        return Option.Some<SimulaContactReport, SimulaNotFinishedResult>(response).Some();
                    }

                case HttpStatusCode.NotFound:
                    {
                        return default;
                    }

                default:
                    {
                        var responseJson = await result.Content.ReadAsStringAsync();
                        throw new Exception($"Kall mot Simula API ga uventet statuskode {result.StatusCode}.{Environment.NewLine}{responseJson}");
                    }
            }
        }

        public async Task<SimulaDeletionsResponse> HentSlettinger(SimulaDeletionsRequest request)
        {
            var requestJson = JsonConvert.SerializeObject(request);

            var response = await _httpClient.PostAsync("fhi/deletions", new StringContent(requestJson, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SimulaDeletionsResponse>(responseJson);
        }

        public async Task<SimulaEventListResponse<SimulaGpsDataEgressEvent>> HentGpsData(SimulaGpsDataEgressRequest request)
        {
            var requestJson = JsonConvert.SerializeObject(request);

            var response = await _httpClient.PostAsync("fhi/fhi-egress", new StringContent(requestJson, Encoding.UTF8, "application/json"));

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return new SimulaEventListResponse<SimulaGpsDataEgressEvent>
                {
                    Events = Enumerable.Empty<SimulaGpsDataEgressEvent>(),
                    FoundInSystem = false,
                    PageNumber = 1,
                    PerPage = request.PerPage,
                    PhoneNumber = request.PhoneNumber,
                    Total = 0
                };
            }

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SimulaEventListResponse<SimulaGpsDataEgressEvent>>(responseJson);
        }

        public async Task<SimulaEventListResponse<SimulaAccessLogEvent>> HentTilgangslogg(SimulaTransparencyRequest request)
        {
            var requestJson = JsonConvert.SerializeObject(request);

            var response = await _httpClient.PostAsync("fhi/fhi-access-log", new StringContent(requestJson, Encoding.UTF8, "application/json"));

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new SimulaEventListResponse<SimulaAccessLogEvent>
                {
                    Events = Enumerable.Empty<SimulaAccessLogEvent>(),
                    FoundInSystem = false,
                    PageNumber = 1,
                    PerPage = request.PerPage,
                    PhoneNumber = request.PhoneNumber,
                    Total = 0
                };
            }

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SimulaEventListResponse<SimulaAccessLogEvent>>(responseJson);
        }
    }


    public static class SimulaApiKlientExtensions
    {
        public static IServiceCollection AddSimulaApiKlient(this IServiceCollection services, IConfiguration config)
        {
            return services.AddSimulaApiKlient<ISimulaEksternApiKlient, SimulaEksternApiKlient>(config);
        }

        public static IServiceCollection AddSimulaApiKlient<TReg, TImp>(this IServiceCollection services, IConfiguration config)
            where TReg : class
            where TImp : class, TReg
        {
            services.AddHttpClient<TReg, TImp>(c =>
            {
                c.BaseAddress = new Uri(config["ApiUrl"]);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var cert = FindCertByThumbprint(config["CertThumbprint"])
                    .ValueOr(() => throw new Exception($"Klarte ikke finne klientsertifikat for autentisering (thumbprint: {config["CertThumbprint"]})."));
                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = SslProtocols.Tls12
                };
                handler.ClientCertificates.Add(cert);
                return handler;
            });
            return services;
        }

        private static Option<X509Certificate2> FindCertByThumbprint(string thumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            var enumerator = certCollection.GetEnumerator();
            var cert = Option.None<X509Certificate2>();
            while (enumerator.MoveNext())
            {
                cert = enumerator.Current.Some();
            }
            store.Close();
            return cert;
        }
    }
}