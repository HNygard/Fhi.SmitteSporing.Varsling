using System.Linq;
using System.Reflection;
using Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Fhi.Smittesporing.Varsling.Intern
{
    public static class ProgramArgs
    {
        public const string UseHttpSys = "--use-httpsys";
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfig) =>
                {
                    loggerConfig
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.WithProperty("Serviceinformasjon", new Serviceinformasjon
                        {
                            Tjenestenavn = hostingContext.Configuration["Tjenestenavn"] ?? "Fhi.Smittesporing.Varsling.Intern",
                            Versjon = Assembly.GetEntryAssembly()?.GetName().Version.ToString()
                        }, destructureObjects: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .SetupForWindowsAuth(args)
                        .UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Registrere hostedservice her slik at de starter etter Configure i stedet for før (mht DB-migrering)
                    var cfgs = hostContext.Configuration.GetSection("Bakgrunnsjobber");
                    services
                        .LeggTilPeriodiskJobb<HenteNyeIndekspasienterJobb, HenteNyeIndekspasienterJobb.Konfig>(cfgs.GetSection("HenteNyeIndekspasienter"))
                        .LeggTilPeriodiskJobb<LastInnSmittekontakterJobb, LastInnSmittekontakterJobb.Konfig>(cfgs.GetSection("LastInnSmittekontakter"))
                        .LeggTilPeriodiskJobb<FjerntUtgatteDataJobb, FjerntUtgatteDataJobb.Konfig>(cfgs.GetSection("FjernUtgatteIndekspasienter"))
                        .LeggTilPeriodiskJobb<SendSmsVarslerJobb, SendSmsVarslerJobb.Konfig>(cfgs.GetSection("SendSmsVarsler"))
                        .LeggTilPeriodiskJobb<OppdaterSmsStatuserJobb, OppdaterSmsStatuserJobb.Konfig>(cfgs.GetSection("OppdaterSmsStatuser"))
                        .LeggTilPeriodiskJobb<SynkroniserSlettingerMotSimulaJobb, SynkroniserSlettingerMotSimulaJobb.Konfig>(cfgs.GetSection("SynkroniserSlettingerMotSimula"));
                });
    }

    public class Serviceinformasjon
    {
        public string Versjon { get; set; }
        public string Tjenestenavn { get; set; }
    }

    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder SetupForWindowsAuth(this IWebHostBuilder builder, string[] args)
        {
            if (args.Contains(ProgramArgs.UseHttpSys))
            {
                // Self hosted
                return builder
                    .ConfigureServices(services =>
                    {
                        services.AddAuthentication(HttpSysDefaults.AuthenticationScheme);
                    })
                    .UseHttpSys(options =>
                    {
                        // Bruker HTTP.sys for å støtte windows autentisering (NTLM) med self hosting
                        options.Authentication.Schemes = AuthenticationSchemes.NTLM | AuthenticationSchemes.Negotiate;
                        options.Authentication.AllowAnonymous = true;
                    });
            }

            // IIS
            return builder.ConfigureServices(services =>
            {
                services.AddAuthentication(IISDefaults.AuthenticationScheme);
            });
        }
    }
}
