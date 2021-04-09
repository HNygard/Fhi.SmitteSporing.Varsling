using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber
{
    public class JobbIntervallKonfig
    {
        public TimeSpan PauseFantArbeid { get; set; } = TimeSpan.FromSeconds(20);
        public TimeSpan PauseIngenArbeid { get; set; } = TimeSpan.FromMinutes(2);
        public TimeSpan PauseUventetFeil { get; set; } = TimeSpan.FromMinutes(10);
    }

    public interface IPeriodiskJobb
    {
        Task<bool> UtforJobb(CancellationToken stoppingToken);
    }

    public interface IPeriodiskJobbKonfig
    {
        JobbIntervallKonfig JobbIntervaller { get; set; }
    }

    public class PeriodiskJobbHostedService<TJobb, TKonfig> : BackgroundService
        where TJobb : IPeriodiskJobb
        where TKonfig : class, IPeriodiskJobbKonfig, new()
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private readonly JobbIntervallKonfig _intervallKonfig;

        public PeriodiskJobbHostedService(IServiceProvider services, ILogger<PeriodiskJobbHostedService<TJobb, TKonfig>> logger, IOptions<TKonfig> konfig)
        {
            _services = services;
            _logger = logger;
            _intervallKonfig = konfig.Value.JobbIntervaller;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    bool foundWork;
                    using (var scope = _services.CreateScope())
                    {
                        var arbeidskontekts = scope.ServiceProvider.GetService<IArbeidskontekst>();
                        arbeidskontekts.SettSystemjobbkontekst<TJobb>();
                        var jobb = scope.ServiceProvider.GetService<TJobb>();
                        foundWork = await jobb.UtforJobb(stoppingToken);
                    }

                    if (foundWork)
                    {
                        _logger.LogInformation($"Gjennomførte kjøring av { typeof(TJobb).Name }, nytt arbeid ble funnet og utført!");
                        await Task.Delay(_intervallKonfig.PauseFantArbeid, stoppingToken);
                    }
                    else
                    {
                        _logger.LogDebug($"Gjennomførte kjøring av { typeof(TJobb).Name }, men intet nytt arbeid ble funnet..");
                        await Task.Delay(_intervallKonfig.PauseIngenArbeid, stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    if (!stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogError(e, $"Uventet feil ved utføring av jobb { typeof(TJobb).Name }. Venter ekstra før neste forsøk.");
                        await Task.Delay(_intervallKonfig.PauseUventetFeil, stoppingToken);
                    }
                }
            }
        }
    }

    public static class PeriodiskJobbExtensions
    {
        public static IServiceCollection LeggTilPeriodiskJobb<TJobb, TJobbKonfig>(this IServiceCollection services, IConfiguration config) 
            where TJobb : class, IPeriodiskJobb
            where TJobbKonfig : class, IPeriodiskJobbKonfig, new()
        {
            if (config["Deaktivert"] == "True")
            {
                return services;
            }
            return services
                .Configure<TJobbKonfig>(config)
                .AddScoped<TJobb>()
                .AddHostedService<PeriodiskJobbHostedService<TJobb, TJobbKonfig>>();
        }
    }
}