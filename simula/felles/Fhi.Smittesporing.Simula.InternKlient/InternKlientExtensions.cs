using System;
using System.Net.Http.Headers;
using Fhi.Smittesporing.Simula.InternKlient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fhi.Smittesporing.Varsling.Eksternetjenester
{
    public class SimulaInternKlientKonfig
    {
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(120);
    }

    public static class InternKlientExtensions
    {
        public static IServiceCollection LeggTilSimulaInternklient(this IServiceCollection services, IConfiguration config)
        {
            var klientKonfig = new SimulaInternKlientKonfig();
            config.Bind(klientKonfig);

            services.AddHttpClient<ISimulaInternKlient, SimulaInternKlient>(c =>
            {
                c.Timeout = klientKonfig.RequestTimeout;
                c.BaseAddress = new Uri(klientKonfig.ApiUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", klientKonfig.ApiKey);
            });

            return services;
        }
    }
}
