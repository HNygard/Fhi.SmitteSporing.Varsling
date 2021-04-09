using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Eksternetjenester.Sms;
using Fhi.Smittesporing.Varsling.Preg;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fhi.Smittesporing.Varsling.Eksternetjenester
{
    public static class EksternetjenesterExtensions
    {
        public static IServiceCollection LeggTilEksternetjeneste(this IServiceCollection services, IConfigurationSection configSection)
        {
            // SMS-integrasjon
            services.Configure<SmsTjenesteKonfig>(configSection.GetSection("Sms"));
            services.AddHttpClient<SmsTjenesteKlient>(c =>
            {
                c.BaseAddress = new Uri(configSection["Sms:ApiUrl"]);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseDefaultCredentials = true,
                PreAuthenticate = true
            });
            services.AddTransient<ISmsTjenesteFacade, SmsTjenesteFacade>();

            // Simula-integrasjon
            services.LeggTilSimulaInternklient(configSection.GetSection("Simula:InternApiKlient"));
            services.Configure<SimulaFacade.Konfig>(configSection.GetSection("Simula:Facade"));
            services.AddScoped<ISimulaFacade, SimulaFacade>();

            // Msis
            services.AddHttpClient<IMsisFacade, MsisFacade>(c =>
            {
                c.BaseAddress = new Uri(configSection["Msis:ApiUrl"]);
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseDefaultCredentials = true,
                PreAuthenticate = true
            });

            // Kontaktinfo
            services.Configure<KontaktInfoFacade.Konfig>(configSection.GetSection("Kontaktinformasjon"));
            services.AddHttpClient<IKontaktInfoFacade, KontaktInfoFacade>(c =>
            {
                c.BaseAddress = new Uri(configSection["Kontaktinformasjon:ApiUrl"]);
                c.DefaultRequestHeaders.Accept.Clear();
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = true
            });

            // PREG
            if (configSection["Preg:Mock"] == "True")
            {
                services.AddScoped<IPregClient, PregClientMock>();
            }
            else
            {
                services.Configure<PregClientOptions>(configSection.GetSection("Preg"));
                services.AddScoped<IPregClient, PregClient>();
            }
            services.AddScoped<IPregFacade, PregFacade>();

            return services;
        }
    }
}
